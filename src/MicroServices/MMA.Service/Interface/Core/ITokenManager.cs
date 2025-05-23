using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MMA.Domain;

namespace MMA.Service
{
    public interface ITokenManager
    {
        Task<string> GenerateUserTokenAsync(
            UserEntity userEntity,
            CTokenType tokenType = CTokenType.Normal,
            DateTimeOffset? expiredDate = null,
            int maxUse = 1);
        Task<bool> VerifyTokenAsync(
            string token,
            CTokenType tokenType = CTokenType.Normal,
            UserEntity? userEntity = null);

        Task<JwtSecurityToken> ValidateIdTokenAsync(string idToken);

        Task<string> GenerateJWTTokenAsync(
            Dictionary<string, object> payload,
            string symmetricSecurityKey,
            string publicKey,
            string algorithm = SecurityAlgorithms.HmacSha256);
    }
}