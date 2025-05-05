namespace MMA.Domain
{
    public class MegaAccountImportDto
    {
        public string AccountName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RecoveryKey { get; set; } = string.Empty;
    }

    public class MegaAccountFileImportDto
    {
        public string OwnerAccount { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public CNodeType NodeType { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Size { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public CFileStatus Status { get; set; }
    }
}