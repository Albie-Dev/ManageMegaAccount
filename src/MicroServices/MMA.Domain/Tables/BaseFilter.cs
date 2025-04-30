namespace MMA.Domain
{
    public class BaseFilter
    {
        public DateTimeOffset? CreatedFromDate { get; set; }
        public DateTimeOffset? CreatedToDate { get; set; }
        public DateTimeOffset? ModifiedFromDate { get; set; }
        public DateTimeOffset? ModifiedToDate { get; set; }
    }
}