using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    public class MovieEntity : BaseEntity
    {
        [StringLength(maximumLength: 500, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        [StringLength(maximumLength: 50, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;
        public CWorkType WorkType { get; set; }
        public CMovieType MovieType { get; set; }
        public CRegionType RegionType { get; set; }
        public CAgencyType AgencyType { get; set; }
        public CGenreType[] GenreTypes { get; set; } = new CGenreType[5];
        public string ImageUrl { get; set; } = string.Empty;
        public DateTimeOffset ReleaseDate { get; set; }
        public string StoragePRoperties { get; set; } = string.Empty;
        [NotMapped]
        public List<StorageProperty> StorageInfos
        {
            get => StoragePRoperties.FromJson<List<StorageProperty>>();
            set => StoragePRoperties = StorageInfos.ToJson();
        }
        public Guid[] ActorIds { get; set; } = new Guid[10];
        public Guid? ActorId { get; set; }
        public Guid? ExactlyActorId { get; set; }
    }

    public class StorageProperty
    {
        public CStorageType StorageType { get; set; }
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RecoveryKey { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ShortLink { get; set; } = string.Empty;
        public double FileSize { get; set; }
        public CFileStatus FileStatus { get; set; }
        public List<StorageProperty> SubFiles { get; set; } = new();
    }
}