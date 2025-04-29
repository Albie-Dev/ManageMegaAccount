using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Service
{
    public class NotificationEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }
        public string Template { get; set; } = string.Empty;
        public DateTimeOffset ReadDate { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? SenderId { get; set; }
        [ForeignKey(nameof(NotificationEntity.OwnerId))]
        [InverseProperty(property: nameof(UserEntity.Notifications))]
        public virtual UserEntity Owner { get; set; } = null!;

        [ForeignKey(nameof(NotificationEntity.SenderId))]
        [InverseProperty(property: nameof(UserEntity.SentNotifications))]
        public virtual UserEntity? Sender { get; set; } = null;
    }
}