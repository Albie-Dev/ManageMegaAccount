using System.ComponentModel.DataAnnotations;

namespace MMA.Domain
{
    public class MegaAccountImportDto
    {
        [Required(ErrorMessage = "AccountName is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "AccountName must be between 3 and 100 characters.")]
        
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "RecoveryKey is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "RecoveryKey must be between 6 and 255 characters.")]
        // [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "RecoveryKey must be alphanumeric.")]
        public string RecoveryKey { get; set; } = string.Empty;

        // [Required(ErrorMessage = "ExpiredDate is required.")]
        // [DataType(DataType.DateTime, ErrorMessage = "ExpiredDate must be a valid date.")]
        // [Range(typeof(DateTimeOffset), "1/1/2000", "12/31/9999", ErrorMessage = "ExpiredDate must be a valid date between 01/01/2000 and 12/31/9999.")]
        public DateTimeOffset? ExpiredDate { get; set; }
    }

    public class MegaAccountFileImportDto
    {
        public string OwnerAccount { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public CNodeType NodeType { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Size { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public CFileStatus Status { get; set; }
        public string Owner { get; set; } = string.Empty;
    }
}