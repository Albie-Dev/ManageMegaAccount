namespace MMA.Domain
{
    public class MegaAccountImportDto
    {
        public string AccountName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RecoveryKey { get; set; } = string.Empty;
    }
}