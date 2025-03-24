namespace MMA.Domain
{
    public class ActorInfoProperty
    {
        public Guid SubActorId { get; private set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public DateOnly DebutDate { get; set; }
        public int Bust { get; set; }
        public int Waist { get; set; }
        public int Hips { get; set; }
        public CCupSizeType CupSizeType { get; set; }
        public CRegionType RegionType { get; set; }
        public int Height { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CMasterStatus Status { get; set; }
    }
}