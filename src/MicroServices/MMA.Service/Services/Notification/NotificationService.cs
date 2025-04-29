using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IDbRepository _repository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            ILogger<NotificationService> logger,
            IDbRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<int> CountMyNewNotificationsAsync(Guid? userId = null)
        {
            var currentUserId = userId.HasValue ? userId.Value : RuntimeContext.CurrentUserId;
            var newNotificationCount = await _repository.Queryable<NotificationEntity>()
                .Where(s => s.OwnerId == currentUserId && s.IsNew)
                .CountAsync();
            return newNotificationCount;
        }

        public async Task<BasePagedResult<NotificationItemModel>> GetNotificationItemModelWithPagingAsync(TableParam<BaseFilter> param)
        {
            var modelState = param.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: modelState.GetErrors());
            }
            var updatedResult = await UpdateViewNewNotificationsAsync(userId: RuntimeContext.CurrentUserId);
            _logger.LogInformation(message: $"Mark notification as viewed. Result = {updatedResult}");
            IQueryable<NotificationEntity> notifications = _repository.Queryable<NotificationEntity>()
                .Include(s => s.Owner)
                .Include(s => s.Sender);
            if (!string.IsNullOrEmpty(param.SearchQuery))
            {
                notifications = notifications.Where(s => s.Title.ToLower().Contains(param.SearchQuery.ToLower()));
            }
            notifications = notifications.OrderByDescending(s => s.CreatedDate);
            var pagedList = await PagedList<NotificationEntity>.ToPagedListAsync(source: notifications, pageNumber: param.PageNumber, pageSize: param.PageSize);
            var selected = pagedList.Select(selector: s => new NotificationItemModel()
            {
                Id = s.Id,
                CreatedDate = s.CreatedDate,
                IsRead = s.IsRead,
                Route = s.Route,
                Thumbnail = s.Thumbnail,
                Title = s.Title,
                OwnerProperty = new UserBaseInfoDto()
                {
                    Avatar = s.Owner.Avatar,
                    Email = s.Owner.Email,
                    FullName = s.Owner.FullName,
                    UserId = s.OwnerId
                },
                SenderProperty = (s.SenderId.HasValue && s.Sender != null) ? 
                new UserBaseInfoDto()
                {
                    Avatar = s.Sender.Avatar,
                    Email = s.Sender.Email,
                    FullName = s.Sender.FullName,
                    UserId = s.SenderId.Value
                } : null
            }).ToList();
            var data = new BasePagedResult<NotificationItemModel>()
            {
                CurrentPage = pagedList.CurrentPage,
                Items = selected,
                PageSize = pagedList.PageSize,
                TotalItems = pagedList.TotalCount,
                TotalPages = pagedList.TotalPages,
                Filter = param.Filter,
            };
            return data;
        }

        public async Task<bool> DeleteNotificationsAsync(List<Guid> notificationIds)
        {
            var notifications = await _repository.GetAsync<NotificationEntity>(predicate: s => notificationIds.Contains(s.Id));
            if (notifications.IsNullOrEmpty())
            {
                return false;
            }
            await _repository.DeleteRangeAsync<NotificationEntity>(entities: notifications);
            return true;
        }

        public async Task<bool> CreateNotificationsAsync(List<CreateNotificationRequestDto> requestDtos)
        {
            var notifications = requestDtos.Adapt<List<NotificationEntity>>();
            if (notifications.IsNullOrEmpty())
            {

            }
            await _repository.AddRangeAsync<NotificationEntity>(entities: notifications, clearTracker: true);
            return true;
        }

        public async Task<bool> UpdateViewNewNotificationsAsync(Guid? userId = null)
        {
            Guid currentUserId = userId.HasValue ? userId.Value : RuntimeContext.CurrentUserId;
            var newNotifications = await _repository.GetAsync<NotificationEntity>(predicate: s => s.OwnerId == currentUserId
                && s.IsNew);
            if (newNotifications.IsNullOrEmpty())
            {
                return false;
            }
            newNotifications.ForEach(s => s.IsNew = false);
            await _repository.UpdateRangeAsync<NotificationEntity>(entities: newNotifications);
            return true;
        }

        public async Task<NotificationDetailDto> GetNotificationByIdAsyn(Guid notificationId)
        {
            bool isUpdated = await MarkNotificationReadAsyn(notificationId: notificationId);
            if (isUpdated)
            {
                _logger.LogInformation($"Notification mark read. Id = {notificationId}");
            }
            var notificationDetail = await _repository.Queryable<NotificationEntity>()
                .Include(s => s.Owner)
                .Include(s => s.Sender)
                .Join(_repository.Queryable<UserEntity>(), nt => nt.CreatedBy, us => us.Id,
                    (nt, us) => new
                    {
                        Notification = nt,
                        Creator = us
                    })
                .Select(nt => new NotificationDetailDto()
                {
                    Id = nt.Notification.Id,
                    IsNew = nt.Notification.IsNew,
                    IsRead = nt.Notification.IsRead,
                    ReadDate = nt.Notification.ReadDate,
                    Template = nt.Notification.Template,
                    Title = nt.Notification.Title,
                    CreatedDate = nt.Notification.CreatedDate,
                    ModifiedDate = nt.Notification.ModifiedDate,
                    Route = nt.Notification.Route,
                    Thumbnail = nt.Notification.Thumbnail,
                    CreatorProperty = new UserBaseInfoDto()
                    {
                        UserId = nt.Notification.CreatedBy,
                        Avatar = nt.Creator.Avatar,
                        Email = nt.Creator.Email,
                        FullName = nt.Creator.FullName
                    },
                    OwnerProperty = new UserBaseInfoDto()
                    {
                        UserId = nt.Notification.OwnerId,
                        Avatar = nt.Notification.Owner.Avatar,
                        Email = nt.Notification.Owner.Email,
                        FullName = nt.Notification.Owner.FullName
                    },
                    SenderProperty = (nt.Notification.SenderId.HasValue && nt.Notification.Sender != null)
                    ? new UserBaseInfoDto()
                    {
                        Avatar = nt.Notification.Sender.Avatar,
                        Email = nt.Notification.Sender.Email,
                        FullName = nt.Notification.Sender.FullName,
                        UserId = nt.Notification.SenderId.Value
                    } : null
                }).FirstOrDefaultAsync();
            if (notificationDetail == null)
            {
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = $"Không tìm thấy thông báo. NotificationId = {notificationId}",
                        ErrorScope = CErrorScope.PageSumarry
                    }
                });
            }
            return notificationDetail;
        }

        public async Task<bool> MarkNotificationReadAsyn(Guid notificationId)
        {
            var notification = await _repository.FindForUpdateAsync<NotificationEntity>(predicate: nt => nt.Id == notificationId);
            if (notification == null)
            {
                throw new MMAException(statusCode: StatusCodes.Status404NotFound, new List<ErrorDetailDto>()
                {
                    new ErrorDetailDto()
                    {
                        Error = $"Không tìm thấy thông báo. NotificationId = {notificationId}",
                        ErrorScope = CErrorScope.PageSumarry
                    }
                });
            }
            notification.IsRead = true;
            notification.IsNew = false;
            await _repository.UpdateAsync<NotificationEntity>(entity: notification, clearTracker: true);
            return true;
        }
    }
}