namespace MMA.Domain
{
    public class MegaAccountFilterProperty : BaseFilter
    {
        public List<Guid> MegaAccountIds { get; set; } = new List<Guid>();
        public DateTimeRageFilterProperty<DateTimeOffset> LastLoginRange { get; set; } = new DateTimeRageFilterProperty<DateTimeOffset>();
        public DateTimeRageFilterProperty<DateTimeOffset> ExpiredDateRange { get; set; } = new DateTimeRageFilterProperty<DateTimeOffset>();
    }
}