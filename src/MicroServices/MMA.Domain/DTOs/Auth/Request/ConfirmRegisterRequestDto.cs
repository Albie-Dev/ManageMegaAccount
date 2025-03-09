namespace MMA.Domain
{
    public class ConfirmRegisterRequestDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}