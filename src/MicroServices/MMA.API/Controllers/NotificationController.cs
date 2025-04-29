using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API.Controllers
{
    [ApiController]
    [Route("api/v1/cet")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("notification/pagingitem")]
        public async Task<IActionResult> GetNotificationItemModelWithPaging(TableParam<BaseFilter> param)
        {
            var result = await _notificationService.GetNotificationItemModelWithPagingAsync(param: param);
            return Ok(new ResponseResult<BasePagedResult<NotificationItemModel>>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpGet("notification/countnew")]
        public async Task<IActionResult> CountNewNotifications()
        {
            var result = await _notificationService.CountMyNewNotificationsAsync();
            return Ok(new ResponseResult<int>()
            {
                Data = result,
                Success = true
            });
        }
    }
}