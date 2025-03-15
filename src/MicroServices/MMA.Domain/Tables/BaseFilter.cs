namespace MMA.Domain
{
    public class BaseFilter
    {
        public DateTimeOffset? CreatedFromDate { get; set; }
        public DateTimeOffset? CreatedToDate { get; set; }
    }
}