namespace MMA.Domain
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = null!;
        public JwtConfig JwtConfig { get; set; } = null!;
        public EmailConfig EmailConfig { get; set; } = null!;
        public ClientApp ClientApp { get; set; } = null!;
        public EndpointConfig EndpointConfig { get; set; } = null!;
        public SocialAuthConfig SocialAuthConfig { get; set; } = null!;

    }
}