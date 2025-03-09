using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MMA.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MMA.Service
{
    public class TokenManager : ITokenManager
    {
        private readonly IDbRepository _repository;
        private readonly ILogger<TokenManager> _logger;

        public TokenManager(
            IDbRepository repository,
            ILogger<TokenManager> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<string> GenerateUserTokenAsync(
            UserEntity userEntity,
            CTokenType tokenType = CTokenType.Normal,
            DateTimeOffset? expiredDate = null,
            int maxUse = 1)
        {
            string token = string.Empty;
            switch (tokenType)
            {
                case CTokenType.PhoneToken:
                    token = GenerateRandomNumericToken(6);
                    break;

                case CTokenType.TwoFactorEnable:
                    token = GenerateRandomNumericToken(6);
                    break;

                case CTokenType.EmailToken:
                    token = Guid.NewGuid().ToString("N");
                    break;

                case CTokenType.DiscountToken:
                    token = $"DISCOUNT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                    break;

                case CTokenType.Normal:
                default:
                    token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    break;
            }

            var userTokenEntity = new UserTokenEntity()
            {
                TokenType = tokenType,
                Token = token,
                MaxUse = maxUse,
                ExpiredDate = expiredDate ?? DateTimeOffset.UtcNow.AddMinutes(30),
                UserId = userEntity.Id
            };
            try
            {
                await _repository.AddAsync<UserTokenEntity>(entity: userTokenEntity, clearTracker: true);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError, errors: new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = $"Đã có lỗi xảy ra trong quá trình tạo Token. TokenType = {tokenType.ToDescription()}",
                        ErrorScope = CErrorScope.PageSumarry,
                        Field = string.Empty
                    }
                });
            }
        }

        public async Task<bool> VerifyTokenAsync(
            string token,
            CTokenType tokenType = CTokenType.Normal,
            UserEntity? userEntity = null)
        {
            try
            {
                // Retrieve the token from the database
                var userTokenEntity = await _repository.FindAsync<UserTokenEntity>(t => t.Token == token && t.TokenType == tokenType);

                if (userTokenEntity == null)
                {
                    _logger.LogWarning($"Token {token} not found in the database.");
                    return false;
                }

                // Check if the token has expired
                if (userTokenEntity.ExpiredDate < DateTimeOffset.UtcNow)
                {
                    _logger.LogWarning($"Token {token} has expired.");
                    return false;
                }

                // Check if the token has exceeded the max usage limit
                if (userTokenEntity.MaxUse <= 0)
                {
                    _logger.LogWarning($"Token {token} has reached the max usage limit.");
                    return false;
                }

                // Optionally, you can check the user entity if provided
                if (userEntity != null && userTokenEntity.UserId != userEntity.Id)
                {
                    _logger.LogWarning($"Token {token} does not belong to the provided user.");
                    return false;
                }

                // If all checks pass, update max use and return true
                userTokenEntity.MaxUse -= 1;
                await _repository.UpdateAsync(userTokenEntity);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying the token.");
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError, errors: new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = $"An error occurred while verifying the token. TokenType = {tokenType.ToDescription()}",
                        ErrorScope = CErrorScope.PageSumarry,
                        Field = string.Empty
                    }
                });
            }
        }


        private string GenerateRandomNumericToken(int length)
        {
            var random = new Random();
            var token = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                token.Append(random.Next(0, 10));
            }
            return token.ToString();
        }


        public async Task<JwtSecurityToken> ValidateIdTokenAsync(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var microsoftAuthConfig = RuntimeContext.AppSettings.SocialAuthConfig.MicrosoftAuthConfig;
            var jsonWebKeySet = await GetMicrosoftPublicKeysAsync(endpoint: microsoftAuthConfig.PublicKeyUrl);
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKeys = jsonWebKeySet.Keys,
                ValidAudience = microsoftAuthConfig.ClientId,
                ValidIssuer = $"https://login.microsoftonline.com/{microsoftAuthConfig.ConsumerTenantId}/v2.0",
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            try
            {
                var principal = handler.ValidateToken(idToken, validationParameters, out validatedToken);
                return (JwtSecurityToken)validatedToken;
            }
            catch (SecurityTokenValidationException ex)
            {
                System.Console.WriteLine(ex.Message);
                throw new UnauthorizedAccessException("Invalid ID token");
            }
        }

        private async Task<JsonWebKeySet> GetMicrosoftPublicKeysAsync(string endpoint)
        {
            using var httpClient = new HttpClient();
            {
                var response = await httpClient.GetStringAsync(endpoint);
                var jsonWebKeySet = response.FromJson<JsonWebKeySet>();
                return jsonWebKeySet;
            }
        }
    }
}