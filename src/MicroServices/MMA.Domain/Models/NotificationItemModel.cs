namespace MMA.Domain
{
    public class NotificationItemModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public string Route { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public UserBaseInfoDto? OwnerProperty { get; set; }
        public UserBaseInfoDto? SenderProperty { get; set; }

    }
}