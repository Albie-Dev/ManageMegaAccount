using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API.Controllers
{
    [ApiController]
    [Route("api/v1/cloud")]
    public class CloudController : ControllerBase
    {
        private readonly IImageKitIOService _imageKitIOService;

        public CloudController(
            IImageKitIOService imageKitIOService
        )
        {
            _imageKitIOService = imageKitIOService;
        }

        [HttpPost("imagekit/generatetoken")]
        public async Task<IActionResult> GetImageKitAuthInfo(ImageKitIOGenerateJWTRequestDto requestDto)
        {
            var result = await _imageKitIOService.GenerateJWTTokenAsync(requestDto);
            return Ok(new ResponseResult<string>()
            {
                Data = result,
                Success = true
            });
        }

        [HttpDelete("imagekit/{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            var result = await _imageKitIOService.DeleteFileAsync(fileId: fileId);
            return Ok(new ResponseResult<bool>()
            {
                Data = result,
                Success = true
            });
        }
    }
}