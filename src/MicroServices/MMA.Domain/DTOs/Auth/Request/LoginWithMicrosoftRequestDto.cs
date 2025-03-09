namespace MMA.Domain
{
    public class LoginWithMicrosoftRequestDto
    {
        public string IdToken { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class LoginWithGoogleRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}