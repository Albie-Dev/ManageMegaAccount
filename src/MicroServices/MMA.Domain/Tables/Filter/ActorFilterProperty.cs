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
        public DateTimeRageFilterProperty<DateOnly> DebutDateRange { get; set; } = new DateTimeRageFilterProperty<DateOnly>();
        public DateTimeRageFilterProperty<DateOnly> DateOfBirthRange { get; set; } = new DateTimeRageFilterProperty<DateOnly>();
        public List<CMasterStatus> Statuses { get; set; } = new List<CMasterStatus>();
    }
}