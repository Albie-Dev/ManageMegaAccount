using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API
{
    [ApiController]
    [Route("/api/v1/movie")]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpPost("actor/paging")]
        public async Task<IActionResult> GetActorWithPaging(TableParam<ActorFilterProperty> tableParam)
        {
            var result = await _actorService.GetActorWithPagingAsync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<ActorDetailDto>>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPost("actor/allinfos")]
        public async Task<IActionResult> GetActorBaseInfo(TableParam<BaseFilter> tableParam)
        {
            var result = await _actorService.GetActorBaseInfoASync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<UserBaseInfoDto>>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpGet("actor/detail/{actorId:guid}")]
        public async Task<IActionResult> GetActorDetail(Guid actorId)
        {
            var result = await _actorService.GetActorDetaiAsync(actorId: actorId);
            return Ok(new ResponseResult<ActorDetailDto>()
            {
                Data = result,
                Success = true,
            }); 
        }

        [HttpPost("actor/create")]
        public async Task<IActionResult> AddActor(CreateActorRequestDto actorRequestDto)
        {
            var result = await _actorService.AddActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPost("actor/update")]
        public async Task<IActionResult> UpdateActor(UpdateActorRequestDto actorRequestDto)
        {
            var result = await _actorService.UpdateActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPost("actor/deactive")]
        public async Task<IActionResult> DeactiveActor(DeactiveActorRequestDto actorRequestDto)
        {
            NotificationResponse result = await _actorService.DeactiveActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            }); 
        }

        [HttpPost("actor/delete")]
        [MMAAuthorized(Permission = CPermissionType.Manage, Resource = CResourceType.AdminUserManagement, Role = CRoleType.Admin)]
        public async Task<IActionResult> DeleteActor(DeleteActorRequestDto actorRequestDto)
        {
            var result = await _actorService.DeleteActorAsync(actorRequestDto: actorRequestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true,
            });
        }

        [HttpPost("actor/export")]
        public async Task<IActionResult> ExportActor(TableParam<ActorFilterProperty> tableParam)
        {
            var currentUser = RuntimeContext.CurrentUser;
            string ownerName = currentUser?.FullName ?? "System";
            var exportResult = await _actorService.ExportActorByExcelTemplateAsyn(tableParam);
            return new FileContentResult(fileContents: exportResult, contentType: "application/octet-stream")
            {
                FileDownloadName = $"Export_Actor_Details_by_{ownerName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx".Trim().Replace(" ", "_")
            };
        }
    }
}