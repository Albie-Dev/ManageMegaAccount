namespace MMA.Domain
{
    public class NotificationResponse
    {
        public string Message { get; set; } = string.Empty;
        public CNotificationDisplayType DisplayType { get; set; }
        public CNotificationLevel Level { get; set; }
        public CNotificationType Type { get; set; }
    }
}