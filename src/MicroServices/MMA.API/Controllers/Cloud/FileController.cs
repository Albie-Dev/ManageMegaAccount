using Microsoft.AspNetCore.Mvc;
using MMA.Domain;
using MMA.Service;

namespace MMA.API
{
    [ApiController]
    [Route("api/v1/cloud")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(
            IFileService fileService
        )
        {
            _fileService = fileService;
        }

        [HttpGet("file/{id}/{cloudType:int}")]
        public async Task<IActionResult> GetFileById(string id, CCloudType cloudType)
        {
            var result = await _fileService.GetFileDetailByIdAsync(fileId: id, cloudType: cloudType);
            return StatusCode(statusCode: result.StatusCode, value: result.JsonResponse);
        }

        [HttpPost("file/{cloudType:int}")]
        public async Task<IActionResult> GetFileByURL([FromRoute] CCloudType cloudType, [FromBody] string url)
        {
            var result = await _fileService.GetFileDetailByUrlAsync(url: url, cloudType: cloudType);
            return StatusCode(statusCode: result.StatusCode, value: result.JsonResponse);
        }
    }
}