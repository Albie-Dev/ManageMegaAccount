using MMA.Service;
using MMA.Domain;
using Microsoft.AspNetCore.Mvc;

namespace MMA.API
{
    [ApiController]
    [Route("/api/v1/cloud")]
    public class MegaAccountController : ControllerBase
    {
        private readonly IMegaAccountService _megaAccountService;

        public MegaAccountController(IMegaAccountService megaAccountService)
        {
            _megaAccountService = megaAccountService;
        }

        [HttpPost("megaaccount/login")]
        public async Task<IActionResult> Login(MegaAccountLoginRequestDto requestDto)
        {
            var result = await _megaAccountService.LoginMegaAccountWithIdAsync(requestDto: requestDto);
            return Ok(new ResponseResult<NotificationResponse>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("megaaccount/paging")]
        public async Task<IActionResult> GetMegaAccountWithPaging(TableParam<MegaAccountFilterProperty> tableParam)
        {
            var result = await _megaAccountService.GetMegaAccountWithPagingAsync(tableParam: tableParam);
            return Ok(new ResponseResult<BasePagedResult<MegaAccountDetailDto>>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpPost("megaaccount/template")]
        public async Task<IActionResult> DownloadMegaAccountImportTemplate()
        {
            var currentUser = RuntimeContext.CurrentUser;
            string ownerName = currentUser?.FullName ?? "System";
            byte[] result = await _megaAccountService.DownloadMegaAccountImportTemplateAsync();
            return new FileContentResult(fileContents: result, contentType: "application/octet-stream")
            {
                FileDownloadName = $"Import_MegaAccount_Template_by_{ownerName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx".Trim().Replace(" ", "_")
            };
        }

        [HttpPost("megaaccount/import")]
        public async Task<IActionResult> ImportMegaAccounts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or missing.");
            }

            using var stream = file.OpenReadStream();
            var result = await _megaAccountService.ImportMegaAccountsAsync(fileStream: stream);
            var currentUser = RuntimeContext.CurrentUser;
            string ownerName = currentUser?.FullName ?? "System";
            return new FileContentResult(fileContents: result, contentType: "application/octet-stream")
            {
                FileDownloadName = $"Import_MegaAccount_Result_by_{ownerName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx".Trim().Replace(" ", "_")
            };
        }
    }
}