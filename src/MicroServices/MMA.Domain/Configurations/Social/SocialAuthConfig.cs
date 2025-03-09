namespace MMA.Domain
{
    public class SocialAuthConfig
    {
        public FacebookAuthConfig FacebookAuthConfig { get; set; } = null!;
        public MicrosoftAuthConfig MicrosoftAuthConfig { get; set; } = null!;
        public GoogleAuthConfig GoogleAuthConfig { get; set; } = null!;
    }
}