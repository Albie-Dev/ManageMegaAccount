namespace MMA.BlazorWasm
{
    public class DateTimePickerModelData
    {
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? EndDateTime { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}