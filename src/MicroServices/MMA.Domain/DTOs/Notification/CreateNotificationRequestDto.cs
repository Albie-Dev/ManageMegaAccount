namespace MMA.Domain
{
    public class CreateNotificationRequestDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }
        public string Template { get; set; } = string.Empty;
        public DateTimeOffset ReadDate { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? SenderId { get; set; }
    }
}