using MMA.Domain;

namespace MMA.Service
{
    public class FileEntity : BaseEntity
    {
        public string FileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Size { get; set; }
        public CFileType Type { get; set; }
        public CCloudType CloudType { get; set; }
        public string PublicUrl { get; set; } = string.Empty;
        public string PrivateUrl { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public VideoFileProperty? VideoFileProperty { get; set; }
    }
}