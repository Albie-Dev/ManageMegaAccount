using MMA.Domain;

namespace MMA.Service
{
    public interface IAuthService
    {
        Task<RedirectResponseDto> GenerateLoginWithGoogleUrlAsync(GenerateGoogleLoginUrlRequestDto requestDto);
        Task<LoginResponseDto> LoginWithGoogleAsync(LoginWithGoogleRequestDto requestDto);
        Task<RedirectResponseDto> GenerateLoginWithMicrosoftUrlAsync(GenerateMicrosoftLoginUrlRequestDto requestDto);
        Task<LoginResponseDto> LoginWithMicrosoftAsync(LoginWithMicrosoftRequestDto requestDto);
        Task<LoginResponseDto> SystemLoginAsync(LoginRequestDto requestDto);
        Task<LoginResponseDto> RefreshAccessTokenAsync(RefreshAccessTokenRequestDto requestDto);
        Task<NotificationResponse> RegisterAsync(RegisterRequestDto requestDto);
        Task<NotificationResponse> ConfirmRegisterAsync(ConfirmRegisterRequestDto requestDto);
        Task<LoginResponseDto> ConfirmTwoFactorAuthenticationAsync(ConfirmTwoFactorAuthenticationRequestDto requestDto);
        Task<bool> TwoFactorAuthVerifyTokenAsync(string token);
        Task<NotificationResponse> ResendRequestVerifyTwoFactorTokenAsync(
            ConfirmTwoFactorAuthenticationRequestDto requestDto);
            
        Task SendEmailConfirmAsync(UserEntity userEntity);
        Task<RedirectResponseDto> LogoutAsync(LogoutRequestDto requestDto);
    }
}