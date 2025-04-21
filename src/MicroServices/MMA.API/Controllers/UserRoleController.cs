using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API.Controllers
{
    [ApiController]
    [Route("api/v1/cet")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(
            IUserRoleService userRoleService
        )
        {
            _userRoleService = userRoleService;
        }

        [HttpGet("userrole/{userId:guid}")]
        public async Task<IActionResult> GetUserRoleResourcePermission(Guid userId)
        {
            List<UserRoleProperty> result = await _userRoleService.GetUserRoleResourcePermissionAsync(userId: userId);
            return Ok(new ResponseResult<List<UserRoleProperty>>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("userrole/update")]
        public async Task<IActionResult> AddUserRoleResourcePermissionsAsync(AddUserRoleRequestDto requestDto)
        {
            var result = await _userRoleService.AddUserRoleResourcePermissionsAsync(requestDto: requestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpGet("userrole/init")]
        public async Task<IActionResult> InitUserRoleResourcePermission()
        {
            var result = await _userRoleService.InitRoleResourcePermission();
            return Ok(new ResponseResult<List<UserRoleProperty>>()
            {
                Data = result,
                Success = true
            });
        }
    }
}