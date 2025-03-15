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
    }
}