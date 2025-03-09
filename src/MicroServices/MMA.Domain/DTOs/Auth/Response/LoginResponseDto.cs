namespace MMA.Domain
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Cookie { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public bool TwoFactorEnable { get; set; }
        public bool EmailConfirmed { get; set; }
        public CAuthType AuthType { get; set; } = CAuthType.System;
    }
}