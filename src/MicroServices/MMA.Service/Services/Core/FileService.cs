using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MMA.Domain;
using Newtonsoft.Json.Linq;

namespace MMA.Service
{
    public class FileService : IFileService
    {
        private readonly IDbRepository _repository;
        private readonly IImageKitIOService _imageKitIOService;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IDbRepository repository,
            IImageKitIOService imageKitIOService,
            ILogger<FileService> logger
        )
        {
            _repository = repository;
            _imageKitIOService = imageKitIOService;
            _logger = logger;
        }

        // public async Task<int> AddFileAsync(string fileId, CCloudType fileType)
        // {
        //     FileEntity? fileEntity = null;
        //     if (fileType == CCloudType.ImageKitIO)
        //     {
        //         var fileResponse = _imageKitIOService.Get
        //         fileResponse
        //     }

        // }

        public async Task<int> CreateFilesAsync(CreateFileRequestDto fileDto)
        {
            switch(fileDto.CloudType)
            {
                case CCloudType.ImageKitIO:
                {
                    List<FileEntity> files = new List<FileEntity>();
                    foreach(var fileId in fileDto.FileIds)
                    {
                        var fileResponse = await _imageKitIOService.GetFileByIdAsync(fileId: fileId);
                        if (fileResponse.StatusCode == StatusCodes.Status200OK)
                        {
                            var file = new FileEntity();

                            JObject jObject = JObject.Parse(fileResponse.JsonResponse);
                            string mime = jObject.GetValue(propertyName: "mime", comparison: StringComparison.OrdinalIgnoreCase)?.ToString() ?? string.Empty;
                            CFileType fileType = FileHelper.GetFileTypeFromMimeType(mimeType: mime);
                            if (fileType == CFileType.Video)
                            {
                                
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"ImageKitIOService CreateFilesAsync. Cannot found with ID = '{fileId}'");
                        }
                    }
                    break;
                }
                
            }
            return 0;
        }

        public async Task<int> DeleteFilesAsync(DeleteFileRequestDto fileDto)
        {
            return await Task.FromResult<int>(0);
        }

        public async Task<(int StatusCode, string JsonResponse)> GetFileDetailByIdAsync(string fileId, CCloudType cloudType)
        {
            switch (cloudType)
            {
                case CCloudType.ImageKitIO: {
                    return await _imageKitIOService.GetFileByIdAsync(fileId: fileId);
                }

                default: break;
            }
            return new ();
        }

        public async Task<(int StatusCode, string JsonResponse)> GetFileDetailByUrlAsync(string url, CCloudType cloudType)
        {
            switch (cloudType)
            {
                case CCloudType.ImageKitIO: {
                    return await _imageKitIOService.GetFileByUrlAsync(fileUrl: url);
                }

                default: break;
            }
            return new ();
        }
    }
}