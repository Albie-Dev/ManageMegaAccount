using MMA.Domain;

namespace MMA.Service
{
    public interface INotificationService
    {
        Task<bool> CreateNotificationsAsync(List<CreateNotificationRequestDto> requestDtos);
        Task<BasePagedResult<NotificationItemModel>> GetNotificationItemModelWithPagingAsync(TableParam<BaseFilter> param);
        Task<bool> UpdateViewNewNotificationsAsync(Guid? userId = null);
        Task<NotificationDetailDto> GetNotificationByIdAsyn(Guid notificationId);
        Task<bool> MarkNotificationReadAsyn(Guid notificationId);
        Task<bool> DeleteNotificationsAsync(List<Guid> notificationIds);
        Task<int> CountMyNewNotificationsAsync(Guid? userId = null);
    }
}