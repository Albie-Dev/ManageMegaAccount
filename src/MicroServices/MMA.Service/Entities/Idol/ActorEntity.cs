using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    public class ActorEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public DateOnly DebutDate { get; set; }
        public int Bust { get; set; }
        public int Waist { get; set; }
        public int Hips { get; set; }
        public CCupSizeType CupSizeType { get; set; }
        public int Height { get; set; }
        public CRegionType RegionType { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ActorInfoProperties { get; set; } = string.Empty;
        [NotMapped]
        public List<ActorInfoProperty> ActorInfos
        {
            get => ActorInfoProperties.FromJson<List<ActorInfoProperty>>();
            set => ActorInfoProperties = ActorInfos.ToJson();
        }
    }
}