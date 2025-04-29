namespace MMA.Domain
{
    public class NotificationDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }
        public string Template { get; set; } = string.Empty;
        public DateTimeOffset ReadDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public UserBaseInfoDto? CreatorProperty { get; set; }
        public UserBaseInfoDto? OwnerProperty { get; set; }
        public UserBaseInfoDto? SenderProperty { get; set; }
    }
}