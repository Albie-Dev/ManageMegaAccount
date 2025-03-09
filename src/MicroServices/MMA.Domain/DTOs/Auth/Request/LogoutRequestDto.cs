namespace MMA.Domain
{
    public class LogoutRequestDto
    {
        public bool IsAllDevice { get; set; }
        public CAuthType AuthType { get; set; }
    }
}