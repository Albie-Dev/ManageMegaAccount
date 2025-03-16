using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API
{
    [ApiController]
    [Route("/api/v1/actor")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpPost("paging")]
        public async Task<IActionResult> GetActorWithPaging(TableParam<ActorFilterProperty> tableParam)
        {
            var result = await _actorService.GetActorWithPagingAsync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<ActorDetailDto>>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPost("allactorinfos")]
        public async Task<IActionResult> GetActorBaseInfo(TableParam<BaseFilter> tableParam)
        {
            var result = await _actorService.GetActorBaseInfoASync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<UserBaseInfoDto>>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpGet("detail/{actorId:guid}")]
        public async Task<IActionResult> GetActorDetail(Guid actorId)
        {
            var result = await _actorService.GetActorDetaiAsync(actorId: actorId);
            return Ok(new ResponseResult<ActorDetailDto>()
            {
                Data = result,
                Success = true,
            }); 
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddActor(CreateActorRequestDto actorRequestDto)
        {
            var result = await _actorService.AddActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateActor(UpdateActorRequestDto actorRequestDto)
        {
            var result = await _actorService.UpdateActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteActor(DeleteActorRequestDto actorRequestDto)
        {
            var result = await _actorService.DeleteActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            });
        }
    }
}