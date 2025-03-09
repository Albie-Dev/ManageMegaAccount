using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    [Table(name: "Cloud_MegaAccount")]
    public class MegaAccountEntity : BaseEntity
    {
        public string AccountName { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string RecoveryKeyHashed { get; set; } = string.Empty;
        public DateTimeOffset LastLogin { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public double TotalFileSize { get; set; }
        public string Files { get; set; } = string.Empty;
        [NotMapped]
        public List<FileProperty> FileProperties
        {
            get => Files.FromJson<List<FileProperty>>();
            set => Files = FileProperties.ToJson();
        }
    }

    public class FileProperty
    {
        public string Id { get; set; } = string.Empty;

        public CNodeType NodeType { get; set; }

        public string Name { get; set; } = string.Empty;

        public long Size { get; set; }

        public DateTime? ModificationDate { get; set; }

        public string Fingerprint { get; set; } = string.Empty;

        public string ParentId { get; set; } = string.Empty;

        public DateTime? CreationDate { get; set; }

        public string Owner { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public CFileStatus Status { get; set; }

        // IFileAttribute[] FileAttributes { get; }
    }
}