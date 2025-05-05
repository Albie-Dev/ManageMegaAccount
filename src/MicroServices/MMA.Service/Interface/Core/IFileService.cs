using MMA.Domain;

namespace MMA.Service
{
    public interface IFileService
    {
        Task<int> CreateFilesAsync(CreateFileRequestDto fileDto);
        Task<int> DeleteFilesAsync(DeleteFileRequestDto fileDto);
        Task<(int StatusCode, string JsonResponse)> GetFileDetailByIdAsync(string fileId, CCloudType cloudType);
        Task<(int StatusCode, string JsonResponse)> GetFileDetailByUrlAsync(string url, CCloudType cloudType);
    }
}