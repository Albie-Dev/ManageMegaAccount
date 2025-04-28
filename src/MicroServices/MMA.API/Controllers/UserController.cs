using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API
{
    [ApiController]
    [Route("api/v1/cet")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService
        )
        {
            _userService = userService;
        }

        [HttpGet("user/baseinfo")]
        [MMAAuthorized]
        public async Task<IActionResult> GetUserBaseInfo()
        {
            var result = await _userService.GetUserBaseInfoAsync();
            return Ok(new ResponseResult<UserBaseInfoDto>()
            {
                Data = result,
                Success = true
            });
        }


        [HttpPost("user/filter")]
        public async Task<IActionResult> GetUsersForFilter(TableParam<BaseFilter> tableParam)
        {
            var result = await _userService.GetUsersForFilterAsync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<UserBaseInfoDto>>()
            {
                Data = result,
                Success = true
            });
        }
    }
}