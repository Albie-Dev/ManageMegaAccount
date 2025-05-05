namespace MMA.Domain
{
    public class BaseFilter
    {
        public DateTimeRageFilterProperty<DateTimeOffset> CreatedDateRange { get; set; } = new DateTimeRageFilterProperty<DateTimeOffset>();
        public DateTimeRageFilterProperty<DateTimeOffset> ModifiedDateRange { get; set; } = new DateTimeRageFilterProperty<DateTimeOffset>();
    }

    public class DateTimeRageFilterProperty<T> where T : struct
    {
        public T? Start { get; set; }
        public T? End { get; set; }
    }
}