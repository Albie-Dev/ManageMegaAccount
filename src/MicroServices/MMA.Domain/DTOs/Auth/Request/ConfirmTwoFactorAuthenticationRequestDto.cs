namespace MMA.Domain
{
    public class ConfirmTwoFactorAuthenticationRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}