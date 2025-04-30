namespace MMA.Domain
{
    public class ActorFilterProperty : BaseFilter
    {
        public List<Guid> ActorIds { get; set; } = new();
        public int? FromBust { get; set; }
        public int? ToBust { get; set; }
        public int? FromWaist { get; set; }
        public int? ToWaist { get; set; }
        public int? FromHips { get; set; }
        public int? ToHips { get; set; }
        public int? FromHeight { get; set; }
        public int? ToHeight { get; set; }
        public List<CCupSizeType> CupSizeTypes { get; set; } = new List<CCupSizeType>();
        public DateOnly? FromDebutDate { get; set; }
        public DateOnly? ToDebutDate { get; set; }
        public DateOnly? FromDateOfBirth { get; set; }
        public DateOnly? ToDateOfBirth { get; set; }
        public List<CMasterStatus> Statuses { get; set; } = new List<CMasterStatus>();
    }
}