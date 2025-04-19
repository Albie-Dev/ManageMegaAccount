using MMA.Domain;
using MMA.Service;
using Microsoft.AspNetCore.Mvc;

namespace MMA.API
{
    [ApiController]
    [Route("api/v1/cet")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            IRoleService roleService,
            ILogger<RoleController> logger
        )
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost("roles/sync")]
        public async Task<IActionResult> SyncNewRole()
        {
            var result = await _roleService.SyncRolesAsync();
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("roles/paging")]
        public async Task<IActionResult> GetWithPaging(TableParam<RoleFilterProperty> tableParam)
        {
            var result = await _roleService.GetWithPagingAsync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<RoleDetailResponseDto>>()
            {
                Data = result,
                Success = true,
            });
        }
    }
}