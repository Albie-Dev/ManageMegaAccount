namespace MMA.Domain
{
    public class GoogleAuthConfig
    {
        public string OAuth2AuthorizationEndpoint { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string GoogleLoginCallBackUrl { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Oauth2TokenEndpoint { get; set; } = string.Empty;
    }
}