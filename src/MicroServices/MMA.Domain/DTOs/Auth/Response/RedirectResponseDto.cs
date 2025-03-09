namespace MMA.Domain
{
    public class RedirectResponseDto
    {
        public string Url { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
    }
}