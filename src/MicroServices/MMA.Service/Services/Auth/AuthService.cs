using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class AuthService : IAuthService
    {
        private readonly IDbRepository _repository;
        private readonly IAuthMethodService _authMethodService;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly ITokenManager _tokenManager;


        public AuthService(
            IDbRepository repository,
            IAuthMethodService authMethodService,
            ILogger<AuthService> logger,
            ITokenManager tokenManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _authMethodService = authMethodService;
            _emailService = emailService;
            _tokenManager = tokenManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

        }

        #region system login
        public async Task<LoginResponseDto> SystemLoginAsync(LoginRequestDto requestDto)
        {
            var modelState = requestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            var userEntity = await _repository.FindAsync<UserEntity>(us => us.Email == requestDto.Email);
            if (userEntity == null)
            {
                modelState.AddError(field: nameof(requestDto.Email), errorMessage: $"Email {requestDto.Email} không tồn tại.");
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }
            // kiểm tra tài khoản bị khóa
            if (userEntity.IsAccountLocked)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Tài khoản của bạn đã bị khóa vô thời hạn. Vui lòng liên hệ Quản trị viên để được hỗ trợ.");
                throw new MMAException(statusCode: StatusCodes.Status423Locked, errors: modelState.GetErrors());
            }
            // kiểm tra logic xác thực email
            if (!userEntity.EmailConfirm)
            {
                await SendEmailConfirmAsync(userEntity: userEntity);
                modelState.AddError(field: string.Empty, errorMessage: $"Tài khoản của bạn chưa xác nhận đăng ký email. Vui lòng truy cập email để xác nhận tài khoản.",
                    errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status303SeeOther, errors: modelState.GetErrors());
            }
            // kiểm tra logic xác thực bước 2
            if (userEntity.TwoFactorEnable)
            {
                await SendEmailTwoFactorAuthenticationAsync(userEntity: userEntity);
                return new LoginResponseDto()
                {
                    AccessToken = string.Empty,
                    Cookie = string.Empty,
                    EmailConfirmed = true,
                    RefreshToken = string.Empty,
                    Session = string.Empty,
                    TwoFactorEnable = true
                };
            }
            if (string.IsNullOrEmpty(userEntity.PasswordHash))
            {
                modelState.AddError(field: nameof(requestDto.Password), errorMessage: $"Vui lòng chọn phương thức đăng nhập với Microsoft/Google. Sau đó truy cập your profile để tạo mới mật khẩu.");
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }
            bool isValidPassword = PasswordHasher.VerifyPassword(password: requestDto.Password, storedHash: userEntity.PasswordHash);
            if (!isValidPassword)
            {
                modelState.AddError(field: nameof(requestDto.Password), errorMessage: $"Mật khẩu không đúng.");
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }
            // generate token hoặc cookie hoặc session
            // -- Generate Token
            string accessToken = await _authMethodService.GenerateAccessTokenAsync(user: userEntity);
            string refreshToken = await _authMethodService.GenerateRefreshTokenAsync(user: userEntity);
            var userTokenEntity = new UserTokenEntity()
            {
                UserId = userEntity.Id,
                TokenType = CTokenType.AuthToken,
                ExpiredDate = DateTimeOffset.UtcNow.AddDays(7),
                Token = accessToken,
                RefreshToken = refreshToken,
            };
            await _repository.AddAsync<UserTokenEntity>(entity: userTokenEntity);
            return new LoginResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                EmailConfirmed = userEntity.EmailConfirm,
                TwoFactorEnable = userEntity.TwoFactorEnable
            };
        }
        #endregion system login


        #region login with google
        public async Task<RedirectResponseDto> GenerateLoginWithGoogleUrlAsync(GenerateGoogleLoginUrlRequestDto requestDto)
        {
            var response = new RedirectResponseDto();
            var currentUser = RuntimeContext.CurrentUser;

            if (currentUser != null)
            {
                response.IsAuthenticated = true;
                response.Url = string.Empty;
                return response;
            }
            var googleAuthConfig = RuntimeContext.AppSettings.SocialAuthConfig.GoogleAuthConfig;
            string loginGoogleUrl = $"{googleAuthConfig.OAuth2AuthorizationEndpoint}?"
                + $"client_id={googleAuthConfig.ClientId}"
                + "&response_type=code"
                + $"&redirect_uri={googleAuthConfig.GoogleLoginCallBackUrl}"
                + "&scope=openid profile email"
                + $"&state={requestDto.State}";
            response.IsAuthenticated = false;
            response.Url = loginGoogleUrl;
            return await Task.FromResult<RedirectResponseDto>(response);
        }

        public async Task<LoginResponseDto> LoginWithGoogleAsync(LoginWithGoogleRequestDto requestDto)
        {
            var errorModel = requestDto.ModelStateValidate();
            try
            {
                if (!errorModel.GetErrors().IsNullOrEmpty())
                {
                    throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: errorModel.GetErrors());
                }
                var httpClient = new HttpClient();
                var googleAuthConfig = RuntimeContext.AppSettings.SocialAuthConfig.GoogleAuthConfig;
                var tokenRequest = new Dictionary<string, string>
                {
                    {"code", requestDto.Code},
                    {"client_id", googleAuthConfig.ClientId},
                    {"client_secret", googleAuthConfig.ClientSecret},
                    {"redirect_uri", googleAuthConfig.GoogleLoginCallBackUrl},
                    {"grant_type", "authorization_code"}
                };

                var response = await httpClient.PostAsync(googleAuthConfig.Oauth2TokenEndpoint,
                    new FormUrlEncodedContent(tokenRequest));
                if (!response.IsSuccessStatusCode)
                {
                    // lấy error message từ api response về
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    errorModel.AddError(field: string.Empty, errorMessage: errorResponse, errorScope: CErrorScope.PageSumarry);
                    throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: errorModel.GetErrors());
                }
                GoogleOAuth2TokenResponse token = (await response.Content.ReadAsStringAsync()).FromJson<GoogleOAuth2TokenResponse>();
                // Xác thực token và lấy thông tin người dùng
                var userInfo = await GetUserInfoFromGoogleAccessTokenAsync(token.AccessToken);
                var userEntity = await _repository.FindAsync<UserEntity>(u => u.Email == userInfo.Email);
                if (userEntity == null)
                {
                    // create new user
                    userEntity = new UserEntity()
                    {
                        Email = userInfo.Email,
                        UserName = userInfo.Email,
                        FullName = userInfo.Name,
                        Avatar = userInfo.Picture,
                        EmailConfirm = true,
                    };
                    await _repository.AddAsync<UserEntity>(entity: userEntity, needSaveChange: true, clearTracker: true);
                    await AddDefaultRoleAsync(userEntity: userEntity);
                }
                else
                {
                    // kiểm tra tài khoản bị khóa
                    if (userEntity.IsAccountLocked)
                    {
                        errorModel.AddError(field: string.Empty, errorMessage: $"Tài khoản của bạn đã bị khóa vô thời hạn. Vui lòng liên hệ Quản trị viên để được hỗ trợ.");
                        throw new MMAException(statusCode: StatusCodes.Status423Locked, errors: errorModel.GetErrors());
                    }
                    // kiểm tra logic xác thực email
                    if (!userEntity.EmailConfirm)
                    {
                        await SendEmailConfirmAsync(userEntity: userEntity);
                        errorModel.AddError(field: string.Empty, errorMessage: $"Tài khoản của bạn chưa xác nhận đăng ký email. Vui lòng truy cập email để xác nhận tài khoản.",
                            errorScope: CErrorScope.FormSummary);
                        throw new MMAException(statusCode: StatusCodes.Status303SeeOther, errors: errorModel.GetErrors());
                    }
                    // kiểm tra logic xác thực bước 2
                    if (userEntity.TwoFactorEnable)
                    {
                        await SendEmailTwoFactorAuthenticationAsync(userEntity: userEntity);
                        return new LoginResponseDto()
                        {
                            AccessToken = string.Empty,
                            Cookie = string.Empty,
                            EmailConfirmed = true,
                            RefreshToken = string.Empty,
                            Session = string.Empty,
                            TwoFactorEnable = true
                        };
                    }
                    if (string.IsNullOrEmpty(userEntity.Avatar))
                    {
                        userEntity.Avatar = userInfo.Picture;
                        await _repository.UpdateAsync<UserEntity>(entity: userEntity, clearTracker: true, needSaveChange: true);
                    }
                }
                // generate token hoặc cookie hoặc session
                // -- Generate Token
                string accessToken = await _authMethodService.GenerateAccessTokenAsync(user: userEntity);
                string refreshToken = await _authMethodService.GenerateRefreshTokenAsync(user: userEntity);
                var userTokenEntity = new UserTokenEntity()
                {
                    UserId = userEntity.Id,
                    TokenType = CTokenType.AuthToken,
                    ExpiredDate = DateTimeOffset.UtcNow.AddDays(7),
                    Token = accessToken,
                    RefreshToken = refreshToken,
                };
                await _repository.AddAsync<UserTokenEntity>(entity: userTokenEntity);
                return new LoginResponseDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    EmailConfirmed = userEntity.EmailConfirm,
                    TwoFactorEnable = userEntity.TwoFactorEnable,
                    AuthType = CAuthType.Google,
                };

            }
            catch (Exception ex)
            {
                errorModel.AddError(field: string.Empty, errorMessage: ex.Message, errorScope: CErrorScope.PageSumarry);
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError, errors: errorModel.GetErrors());
            }
        }

        private async Task<GoogleUserInfo> GetUserInfoFromGoogleAccessTokenAsync(string accessToken)
        {
            var httpClient = new HttpClient();
            try
            {
                // Đặt Authorization header với access_token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Endpoint để lấy thông tin người dùng từ Google
                var userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

                // Gửi GET request đến endpoint
                var response = await httpClient.GetAsync(userInfoEndpoint);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch user info from Google. StatusCode: {response.StatusCode}");
                }

                // Chuyển đổi response thành GoogleUserInfo
                var userInfoJson = await response.Content.ReadAsStringAsync();
                return userInfoJson.FromJson<GoogleUserInfo>();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching user info: {ex.Message}");
            }
        }

        #endregion login with google

        #region Login with microsoft
        public async Task<LoginResponseDto> LoginWithMicrosoftAsync(LoginWithMicrosoftRequestDto requestDto)
        {
            var errors = requestDto.ModelStateValidate();
            var tokenModel = await _tokenManager.ValidateIdTokenAsync(idToken: requestDto.IdToken);
            string email = tokenModel.Payload["preferred_username"]?.ToString() ?? string.Empty;
            string name = tokenModel.Payload["name"]?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(email))
            {
                errors.AddError(field: string.Empty, errorMessage: "Thông tin xác thực không hợp lệ.", errorScope: CErrorScope.RedirectPage);
                throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: errors.GetErrors());
            }
            var userEntity = await _repository.FindAsync<UserEntity>(u => u.Email == email);
            if (userEntity == null)
            {
                // create new user
                userEntity = new UserEntity()
                {
                    Email = email,
                    UserName = email,
                    FullName = name,
                    EmailConfirm = true,
                };
                await _repository.AddAsync<UserEntity>(entity: userEntity, needSaveChange: true, clearTracker: true);
                await AddDefaultRoleAsync(userEntity: userEntity);
            }
            else
            {
                // kiểm tra tài khoản bị khóa
                if (userEntity.IsAccountLocked)
                {
                    errors.AddError(field: string.Empty, errorMessage: $"Tài khoản của bạn đã bị khóa vô thời hạn. Vui lòng liên hệ Quản trị viên để được hỗ trợ.");
                    throw new MMAException(statusCode: StatusCodes.Status423Locked, errors: errors.GetErrors());
                }
                // kiểm tra logic xác thực email
                if (!userEntity.EmailConfirm)
                {
                    await SendEmailConfirmAsync(userEntity: userEntity);
                    errors.AddError(field: string.Empty, errorMessage: $"Tài khoản của bạn chưa xác nhận đăng ký email. Vui lòng truy cập email để xác nhận tài khoản.",
                        errorScope: CErrorScope.FormSummary);
                    throw new MMAException(statusCode: StatusCodes.Status303SeeOther, errors: errors.GetErrors());
                }
                // kiểm tra logic xác thực bước 2
                if (userEntity.TwoFactorEnable)
                {
                    await SendEmailTwoFactorAuthenticationAsync(userEntity: userEntity);
                    return new LoginResponseDto()
                    {
                        AccessToken = string.Empty,
                        Cookie = string.Empty,
                        EmailConfirmed = true,
                        RefreshToken = string.Empty,
                        Session = string.Empty,
                        TwoFactorEnable = true
                    };
                }
            }
            // generate token hoặc cookie hoặc session
            // -- Generate Token
            string accessToken = await _authMethodService.GenerateAccessTokenAsync(user: userEntity);
            string refreshToken = await _authMethodService.GenerateRefreshTokenAsync(user: userEntity);
            var userTokenEntity = new UserTokenEntity()
            {
                UserId = userEntity.Id,
                TokenType = CTokenType.AuthToken,
                ExpiredDate = DateTimeOffset.UtcNow.AddDays(7),
                Token = accessToken,
                RefreshToken = refreshToken,
            };
            await _repository.AddAsync<UserTokenEntity>(entity: userTokenEntity);
            return new LoginResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                EmailConfirmed = userEntity.EmailConfirm,
                TwoFactorEnable = userEntity.TwoFactorEnable,
                AuthType = CAuthType.Microsoft,
            };
        }
        public async Task<RedirectResponseDto> GenerateLoginWithMicrosoftUrlAsync(GenerateMicrosoftLoginUrlRequestDto requestDto)
        {
            var response = new RedirectResponseDto();
            var currentUser = RuntimeContext.CurrentUser;

            if (currentUser != null)
            {
                response.IsAuthenticated = true;
                response.Url = string.Empty;
                return response;
            }
            var microsoftAuthConfig = RuntimeContext.AppSettings.SocialAuthConfig.MicrosoftAuthConfig;
            string loginMicrosoftUrl = $"{microsoftAuthConfig.OAuth2AuthorizationEndpoint}?"
                + $"client_id={microsoftAuthConfig.ClientId}"
                + "&response_type=id_token"
                + $"&redirect_uri={microsoftAuthConfig.MicrosoftLoginCallBackUrl}"
                + "&scope=openid profile User.Read"
                + $"&state={requestDto.State}"
                + "&nonce=678910";
            response.IsAuthenticated = false;
            response.Url = loginMicrosoftUrl;
            return await Task.FromResult<RedirectResponseDto>(response);
        }
        #endregion Login with microsoft


        #region refresh token
        public async Task<LoginResponseDto> RefreshAccessTokenAsync(RefreshAccessTokenRequestDto requestDto)
        {
            var modelState = requestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            var userToken = await _repository.FindAsync<UserTokenEntity>(s => s.Token == requestDto.AccessToken
                && s.RefreshToken == requestDto.RefreshToken && s.TokenType == CTokenType.AuthToken);
            if (userToken == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy token",
                    errorScope: CErrorScope.RedirectToLoginPage);
                throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: modelState.GetErrors());
            }
            if (userToken.ExpiredDate <= DateTimeOffset.UtcNow)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Token đã hết thời gian hiệu lực. Exired = {userToken.ExpiredDate.ToLocalTime()}",
                    errorScope: CErrorScope.RedirectToLoginPage);
                throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: modelState.GetErrors());
            }
            var userEntity = await _repository.FindAsync<UserEntity>(us => us.Id == userToken.UserId);
            if (userEntity == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy người dùng.",
                    errorScope: CErrorScope.RedirectToLoginPage);
                throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: modelState.GetErrors());
            }
            string accessToken = await _authMethodService.GenerateAccessTokenAsync(user: userEntity);
            userToken.Token = accessToken;
            await _repository.UpdateAsync<UserTokenEntity>(entity: userToken, clearTracker: true);
            return new LoginResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = userToken.RefreshToken,
                EmailConfirmed = userEntity.EmailConfirm,
                TwoFactorEnable = userEntity.TwoFactorEnable
            };
        }
        #endregion refresh token

        #region register
        public async Task<NotificationResponse> RegisterAsync(RegisterRequestDto requestDto)
        {
            var modelState = requestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }


            var userEntity = await _repository.FindAsync<UserEntity>(us => us.Email == requestDto.Email);
            if (userEntity != null)
            {
                modelState.AddError(field: nameof(requestDto.Email), errorMessage: $"Email {requestDto.Email} đã tồn tại.");
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest,
                    errors: modelState.GetErrors());
            }

            userEntity = new UserEntity()
            {
                Email = requestDto.Email,
                UserName = requestDto.Email,
                FullName = requestDto.FullName
            };
            userEntity.PasswordHash = PasswordHasher.HashPassword(password: requestDto.Password);
            userEntity.EmailConfirm = false;
            try
            {
                await _repository.AddAsync<UserEntity>(entity: userEntity, clearTracker: true, needSaveChange: true);
                await AddDefaultRoleAsync(userEntity: userEntity);
                await SendEmailConfirmAsync(userEntity: userEntity);
                return new NotificationResponse()
                {
                    DisplayType = CNotificationDisplayType.Redirect,
                    Level = CNotificationLevel.Info,
                    Message = $"Tài khoản được tạo thành công. Vui lòng truy cập email của bạn để hoàn tất xác thực tài khoản.",
                    Type = CNotificationType.Register
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddError(field: string.Empty, errorMessage: "Đã có lỗi xảy ra trong quá trình đăng ký người dùng.",
                    errorScope: CErrorScope.PageSumarry);
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError,
                    errors: modelState.GetErrors());
            }
        }

        private async Task AddDefaultRoleAsync(UserEntity userEntity)
        {
            // need set default roles
            var roles = await _repository.GetAsync<RoleEntity>();
            if (roles.IsNullOrEmpty())
            {
                foreach (CRoleType role in Enum.GetValues(typeof(CRoleType)))
                {
                    if (role != CRoleType.None)
                    {
                        roles.Add(new RoleEntity()
                        {
                            RoleName = role.ToString(),
                            RoleType = role,
                        });
                    }
                }
                await _repository.AddRangeAsync<RoleEntity>(entities: roles, clearTracker: true, needSaveChange: true);
            }
            // add user role
            var rolePermissions = new List<RolePermission>()
                {
                    new RolePermission()
                    {
                        PermissionTypes = new List<CPermissionType>()
                        {
                            CPermissionType.Read
                        },
                        ResourceType = CResourceType.User
                    }
                };
            var userRoleEntity = new UserRoleEntity()
            {
                UserId = userEntity.Id,
                RoleId = roles.Find(s => s.RoleType == CRoleType.Client)?.Id ?? Guid.Empty,
                RolePermissionProperty = rolePermissions.ToJson()
            };
            await _repository.AddAsync<UserRoleEntity>(entity: userRoleEntity, clearTracker: true, needSaveChange: true);
        }
        #endregion register

        #region confirm register
        public async Task<NotificationResponse> ConfirmRegisterAsync(ConfirmRegisterRequestDto requestDto)
        {
            var modelState = requestDto.ModelStateValidate();
            var response = new NotificationResponse()
            {
                DisplayType = CNotificationDisplayType.Redirect,
                Type = CNotificationType.Register,
                Level = CNotificationLevel.Error
            };
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                response.Message = modelState.GetErrors().Select(s => s.Error).ToList().ToMultilineString();
                return response;
            }

            var userEntity = await _repository.FindAsync<UserEntity>(s => s.Id == requestDto.UserId);
            if (userEntity == null)
            {
                response.Message = $"Không tìm thấy người dùng.";
                return response;
            }
            var userToken = await _repository.FindAsync<UserTokenEntity>(s => s.UserId == requestDto.UserId
                && s.Token == requestDto.Token && s.TokenType == CTokenType.EmailToken);
            if (userToken == null)
            {
                response.Message = $"Không tìm thấy Token = {requestDto.Token}.";
                return response;
            }
            if (userToken.ExpiredDate <= DateTimeOffset.UtcNow)
            {
                response.Message = $"Token = {requestDto.Token} đã hết thời gian hiệu lực.";
                return response;
            }
            if (userToken.MaxUse < 1)
            {
                response.Message = $"Token = {requestDto.Token} đã được sử dụng.";
                return response;
            }
            if (userToken.IsRevoked)
            {
                response.Message = $"Token = {requestDto.Token} đã bị thu hồi.";
                return response;
            }
            await _repository.ActionInTransaction(async () =>
            {
                try
                {
                    userEntity.EmailConfirm = true;
                    userToken.IsRevoked = true;
                    userToken.MaxUse = 0;
                    await _repository.UpdateAsync<UserEntity>(entity: userEntity, clearTracker: true, needSaveChange: false);
                    await _repository.UpdateAsync<UserTokenEntity>(entity: userToken, clearTracker: true);
                    response.Message = "Tài khoản của bạn đã được xác thực thành công.";
                    response.Level = CNotificationLevel.Success;
                }
                catch (Exception ex)
                {
                    response.Message = $"{ex.Message}";
                }
            });
            return response;
        }
        #endregion confirm register

        #region confirm two-factor authentication
        public async Task<LoginResponseDto> ConfirmTwoFactorAuthenticationAsync(ConfirmTwoFactorAuthenticationRequestDto requestDto)
        {
            var response = new LoginResponseDto();
            var modelState = requestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: modelState.GetErrors());
            }
            var userEntity = await _repository.FindAsync<UserEntity>(us => us.Email == requestDto.Email);
            if (userEntity == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Không tìm thấy người dùng.", errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, errors: modelState.GetErrors());
            }
            var userToken = await _repository.FindAsync<UserTokenEntity>(s => s.UserId == userEntity.Id && s.Token == requestDto.Token
                && s.TokenType == CTokenType.TwoFactorEnable);
            if (userToken == null)
            {
                modelState.AddError(field: string.Empty, errorMessage: $"Token = {requestDto.Token} không tồn tại", errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, errors: modelState.GetErrors());
            }
            if (userToken.ExpiredDate <= DateTimeOffset.UtcNow)
            {
                modelState.AddError(field: string.Empty, $"Token = {requestDto.Token} đã hết thời gian hiệu lực.", errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: modelState.GetErrors()); ;
            }
            if (userToken.MaxUse < 1)
            {
                modelState.AddError(field: string.Empty, $"Token = {requestDto.Token} đã được sử dụng.", errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: modelState.GetErrors()); ;
            }
            if (userToken.IsRevoked)
            {
                modelState.AddError(field: string.Empty, $"Token = {requestDto.Token} đã bị thu hồi.", errorScope: CErrorScope.FormSummary);
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: modelState.GetErrors());
            }
            await _repository.ActionInTransaction(async () =>
            {
                try
                {
                    userToken.IsRevoked = true;
                    userToken.MaxUse = 0;
                    await _repository.UpdateAsync<UserTokenEntity>(entity: userToken, clearTracker: true, needSaveChange: false);
                    var accessToken = await _authMethodService.GenerateAccessTokenAsync(user: userEntity);
                    var refreshToken = await _authMethodService.GenerateRefreshTokenAsync(user: userEntity);
                    var userTokenEntity = new UserTokenEntity()
                    {
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        ExpiredDate = DateTimeOffset.UtcNow.AddDays(7),
                        TokenType = CTokenType.AuthToken,
                        UserId = userEntity.Id
                    };
                    await _repository.AddAsync<UserTokenEntity>(entity: userTokenEntity, clearTracker: true, needSaveChange: true);
                    response.TwoFactorEnable = false;
                    response.EmailConfirmed = userEntity.EmailConfirm;
                    response.AccessToken = accessToken;
                    response.RefreshToken = refreshToken;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            });
            return response;
        }
        #endregion confirm two-factor authentication

        #region logout
        public async Task<RedirectResponseDto> LogoutAsync(LogoutRequestDto requestDto)
        {
            var errorModel = requestDto.ModelStateValidate();
            var response = new RedirectResponseDto();
            if (!errorModel.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: errorModel.GetErrors());
            }
            var currentUserId = RuntimeContext.CurrentUserId;
            var currentAccessToken = RuntimeContext.CurrentAccessToken;
            if (currentUserId == Guid.Empty || currentAccessToken == null)
            {
                errorModel.AddError(field: string.Empty, errorMessage: "Authentication failed!", errorScope: CErrorScope.PageSumarry);
                throw new MMAException(statusCode: StatusCodes.Status401Unauthorized, errors: errorModel.GetErrors());
            }
            if (requestDto.AuthType == CAuthType.Microsoft)
            {
                var microsoftAuthConfig = RuntimeContext.AppSettings.SocialAuthConfig.MicrosoftAuthConfig;
                string logoutMicrosoftUrl = $"{microsoftAuthConfig.OAuth2LogoutEndpoint}?"
                    + $"post_logout_redirect_uri={microsoftAuthConfig.PostLogoutRedirectUri}"
                    + $"&logout_hint={RuntimeContext.CurrentUser?.Email}";
                response.Url = logoutMicrosoftUrl;
            }
            var userTokenEntity = await _repository.FindAsync<UserTokenEntity>(urt => urt.UserId == currentUserId
                && urt.Token == currentAccessToken
                && urt.TokenType == CTokenType.AuthToken);
            if (userTokenEntity == null)
            {
                return response;
            }
            var revokedUserTokens = requestDto.IsAllDevice ? await _repository.GetAsync<UserTokenEntity>(urt =>
                urt.UserId == currentUserId
                && urt.TokenType == CTokenType.AuthToken
                && !urt.IsRevoked && urt.ExpiredDate > DateTimeOffset.UtcNow) : new List<UserTokenEntity>() { userTokenEntity };
            var now = DateTimeOffset.UtcNow;
            revokedUserTokens.ForEach(item =>
            {
                item.IsRevoked = true;
                item.ExpiredDate = now;
            });
            await _repository.UpdateRangeAsync(entities: revokedUserTokens);
            return response;
        }
        #endregion logout

        #region send email
        public async Task SendEmailTwoFactorAuthenticationAsync(UserEntity userEntity)
        {
            var userTokens = await _repository.GetAsync<UserTokenEntity>(s => s.UserId == userEntity.Id
                && s.ExpiredDate >= DateTimeOffset.UtcNow && !s.IsRevoked && s.MaxUse > 0
                && s.TokenType == CTokenType.TwoFactorEnable);
            var userTokenExist = userTokens.OrderByDescending(s => s.ExpiredDate).FirstOrDefault();
            if (userTokenExist != null)
            {
                throw new MMAException(statusCode: StatusCodes.Status409Conflict,
                errors: new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = $"Mã xác nhận đã được gửi đến email của bạn. Nếu không nhận được mã vui lòng gửi yêu cầu mới sau {userTokenExist.ExpiredDate.ToLocalTime()}",
                        ErrorScope = CErrorScope.FormSummary,
                        Field = string.Empty
                    }
                });
            }
            var expireTime = DateTimeOffset.UtcNow.AddMinutes(5);
            var token = await _tokenManager.GenerateUserTokenAsync(userEntity: userEntity, tokenType: CTokenType.TwoFactorEnable,
                expiredDate: expireTime, maxUse: 1);
            var clientInfo = RuntimeContext.AppSettings.ClientApp;
            var replaceModel = new TwoFactorAuthenticationTemplateModel()
            {
                Token = token,
                CustomerName = userEntity.FullName,
                ExpiredTime = expireTime.ToLocalTime().ToString(),
                OwnerName = clientInfo.OwnerName,
                CompanyName = clientInfo.CompanyName,
                Address = clientInfo.Address,
                OwnerPhone = clientInfo.OwnerPhone
            };
            await _emailService.SendEmailAsync(email: userEntity.Email, subject: "XÁC THỰC BƯỚC 2", htmlTemplate: string.Empty,
                fileTemplateName: TemplateFileNameConstant.Auth_TwoFactor, replaceProperty: replaceModel,
                emailProviderType: CEmailProviderType.Gmail, serviceType: CMicroserviceType.CET);
        }

        public async Task SendEmailConfirmAsync(UserEntity userEntity)
        {
            var userTokens = await _repository.GetAsync<UserTokenEntity>(ust => ust.ExpiredDate >= DateTime.UtcNow
                && ust.MaxUse > 0 && !ust.IsRevoked && ust.TokenType == CTokenType.EmailToken);
            var userToken = userTokens.OrderByDescending(s => s.ExpiredDate).FirstOrDefault();
            if (userToken != null)
            {
                throw new MMAException(statusCode: StatusCodes.Status409Conflict,
                errors: new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = $"Bạn cần truy cập Email của mình để xác thực tài khoản. Nếu vẫn không nhận được email yêu cầu xác thực vui lòng gửi lại yêu cầu sau : {userToken.ExpiredDate.ToLocalTime()}",
                        ErrorScope = CErrorScope.FormSummary,
                        Field = string.Empty
                    }
                });
            }
            var token = await _tokenManager.GenerateUserTokenAsync(userEntity: userEntity, tokenType: CTokenType.EmailToken,
                expiredDate: DateTimeOffset.UtcNow.AddMinutes(15), maxUse: 1);
            var confirmationLink = LinkHelper.GenerateUrlWithQueryParams(endpoint: RuntimeContext.AppSettings.EndpointConfig.MMA_API,
                routePath: RouteConstant.Auth_Register_Email_Confirm,
                paramObj: new EmailConfirmParam()
                {
                    Token = token,
                    UserId = userEntity.Id.ToString()
                }).ToString();
            var clientInfo = RuntimeContext.AppSettings.ClientApp;
            var emailReplaceProperty = new ConfirmEmailTemplateModel
            {
                ConfirmationLink = confirmationLink,
                CustomerName = userEntity.FullName,
                ReceiverEmail = userEntity.Email,
                CompanyName = clientInfo.CompanyName,
                Address = clientInfo.Address,
                OwnerName = clientInfo.OwnerName,
                OwnerPhone = clientInfo.OwnerPhone
            };
            await _emailService.SendEmailAsync(email: userEntity.Email, subject: "XÁC THỰC ĐĂNG KÝ NGƯỜI DÙNG", htmlTemplate: string.Empty,
                fileTemplateName: TemplateFileNameConstant.Auth_ConfirmRegister, replaceProperty: emailReplaceProperty,
                emailProviderType: CEmailProviderType.Gmail, serviceType: CMicroserviceType.CET);
        }
        #endregion send email
    }
}