namespace MMA.Domain
{
    public class TwoFactorAuthenticationTemplateModel : BaseEmailTemplateModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ExpiredTime { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}