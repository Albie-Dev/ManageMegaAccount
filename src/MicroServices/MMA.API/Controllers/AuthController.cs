using System.IdentityModel.Tokens.Jwt;
using MMA.Service;
using MMA.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MMA.API
{
    [ApiController]
    [Route("api/v1/cet")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost(EndpointConstant.CET_Auth_Logout)]
        [Authorize]
        public async Task<IActionResult> LogOut(LogoutRequestDto requestDto)
        {
            var response = await _authService.LogoutAsync(requestDto: requestDto);
            return Ok(new ResponseResult<RedirectResponseDto>()
            {
                Data = response,
                Success = true
            });
        }

        [HttpPost(EndpointConstant.CET_Auth_Microsoft_Generate_Login_Url)]
        public async Task<IActionResult> GenerateMicrosoftLoginUrl(GenerateMicrosoftLoginUrlRequestDto requestDto)
        {
            var result = await _authService.GenerateLoginWithMicrosoftUrlAsync(requestDto: requestDto);
            return Ok(new ResponseResult<RedirectResponseDto>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost(EndpointConstant.CET_Auth_Google_Generate_Login_Url)]
        public async Task<IActionResult> GenerateGoogleLoginUrl(GenerateGoogleLoginUrlRequestDto requestDto)
        {
            var result = await _authService.GenerateLoginWithGoogleUrlAsync(requestDto: requestDto);
            return Ok(new ResponseResult<RedirectResponseDto>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost(EndpointConstant.CET_Auth_Google_Login)]
        public async Task<IActionResult> GoogleLoginAsync(LoginWithGoogleRequestDto requestDto)
        {
            var response = await _authService.LoginWithGoogleAsync(requestDto: requestDto);
            return Ok(new ResponseResult<LoginResponseDto>()
            {
                Data = response,
                Success = true
            });
        }


        [HttpPost(EndpointConstant.CET_Auth_Microsoft_Login)]
        public async Task<IActionResult> MicrosoftLogin(LoginWithMicrosoftRequestDto requestDto)
        {
            var response = await _authService.LoginWithMicrosoftAsync(requestDto: requestDto);
            return Ok(new ResponseResult<LoginResponseDto>()
            {
                Data = response,
                Success = true
            });
        }

        [HttpPost("auth/systemlogin")]
        public async Task<IActionResult> SystemLogin(LoginRequestDto requestDto)
        {
            var loginResponse = await _authService.SystemLoginAsync(requestDto: requestDto);
            return Ok(new ResponseResult<LoginResponseDto>()
            {
                Data = loginResponse,
                Success = true
            });
        }

        [HttpPost("auth/refreshtoken")]
        public async Task<IActionResult> RefreshAccessToken(RefreshAccessTokenRequestDto requestDto)
        {
            var loginResponse = await _authService.RefreshAccessTokenAsync(requestDto: requestDto);
            return Ok(new ResponseResult<LoginResponseDto>()
            {
                Data = loginResponse,
                Success = true
            });
        }


        [HttpPost("auth/register")]
        public async Task<IActionResult> Register(RegisterRequestDto requestDto)
        {
            var registerResponse = await _authService.RegisterAsync(requestDto: requestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = registerResponse,
                Success = true
            });
        }

        [HttpGet("auth/register/confirm")]
        public async Task<IActionResult> ConfirmRegister([FromQuery] ConfirmRegisterRequestDto requestDto)
        {
            var result = await _authService.ConfirmRegisterAsync(requestDto: requestDto);
            return StatusCode(statusCode: result.Level == CNotificationLevel.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                value: new ResponseResult<NotificationResponse>()
                {
                    Data = result,
                    Success = true
                });
        }


        [HttpPost("auth/twofactor/confirm")]
        public async Task<IActionResult> ConfirmTwoFactorAuthentication(ConfirmTwoFactorAuthenticationRequestDto requestDto)
        {
            var result = await _authService.ConfirmTwoFactorAuthenticationAsync(requestDto: requestDto);
            return StatusCode(statusCode: StatusCodes.Status200OK, value: new ResponseResult<LoginResponseDto>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("auth/twofactor/verifytoken")]
        public async Task<IActionResult> TwoFactorAuthVerifyToken(TwoFactorVerifyTokenRequestDto requestDto)
        {
            bool result = await _authService.TwoFactorAuthVerifyTokenAsync(token: requestDto.Token);
            return Ok(new ResponseResult<bool>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("auth/twofactor/resend")]
        public async Task<IActionResult> ResendRequestVerifyTwoFactorToken(ConfirmTwoFactorAuthenticationRequestDto requestDto)
        {
            var result = await _authService.ResendRequestVerifyTwoFactorTokenAsync(requestDto: requestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpGet("auth/protected")]
        [MMAAuthorized(CRoleType.Client, CResourceType.User, CPermissionType.Read)]
        public async Task<IActionResult> TestAuth()
        {
            var user = RuntimeContext.CurrentUser;
            await Task.CompletedTask;
            return Ok(new ResponseResult<string>()
            {
                Data = "OK",
                Success = true
            });
        }
    }
}