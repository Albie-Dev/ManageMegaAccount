namespace MMA.Domain
{
    public class BaseActorInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public DateOnly DebutDate { get; set; }
        public int Burt { get; set; }
        public int Waist { get; set; }
        public int Hips { get; set; }
        public CCupSizeType CupsizeType { get; set; }
        public CRegionType RegionType { get; set; }
        public int Height { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CMasterStatus Status { get; set; }
        public List<ActorInfoProperty> ActorInfos { get; set; } = new();
    }
}