namespace MMA.Domain
{
    public class GenerateMicrosoftLoginUrlRequestDto
    {
        public string State { get; set; } = string.Empty;
    }

    public class GenerateGoogleLoginUrlRequestDto
    {
        public string State { get; set; } = string.Empty;
    }
}