using MMA.Service;
using MMA.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MMA.API
{
    [ApiController]
    [Route("/api/v1/cet")]
    public class MegaController : ControllerBase
    {
        private readonly IMegaService _megaService;

        public MegaController(IMegaService megaService)
        {
            _megaService = megaService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginDto)
        {
            await _megaService.MegaLoginAsync(requestDto: loginDto);
            return Ok();
        }

        [HttpPost("megaaccount/paging")]
        public async Task<IActionResult> GetMegaAccountWithPaging(TableParam<MegaAccountFilterProperty> tableParam)
        {
            var result = await _megaService.GetMegaAccountWithPagingAsync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<MegaAccountDetailDto>>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("megaaccount/import")]
        public async Task<IActionResult> ImportMegaAccounts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or missing.");
            }

            using var stream = file.OpenReadStream();
            NotificationResponse result = await _megaService.ImportMegaAccountsAsync(fileStream: stream);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true
            });
        }
    }
}