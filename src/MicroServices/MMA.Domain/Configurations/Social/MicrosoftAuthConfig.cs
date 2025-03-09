namespace MMA.Domain
{
    public class MicrosoftAuthConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string SecretId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string ConsumerTenantId { get; set; } = string.Empty;
        public string ObjectId { get; set; } = string.Empty;
        public string OAuth2AuthorizationEndpoint { get; set; } = string.Empty;
        public string OAuth2TokenEndpoint { get; set; } = string.Empty;
        public string OAuth2LogoutEndpoint { get; set; } = string.Empty;
        public string PostLogoutRedirectUri { get; set; } = string.Empty;
        public string PublicKeyUrl { get; set; } = string.Empty;
        public string MicrosoftLoginCallBackUrl { get; set; } = string.Empty;
    }
}