namespace MMA.Domain
{
    public class EmailConfirmParam
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}