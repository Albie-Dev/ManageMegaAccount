using MMA.Domain;

namespace MMA.Service
{
    public interface IImageKitIOService
    {
        Task<string> GenerateJWTTokenAsync(ImageKitIOGenerateJWTRequestDto requestDto);
        Task<(int StatusCode, string JsonResponse)> GetFileByIdAsync(string fileId);
        Task<(int StatusCode, string JsonResponse)> GetFileByUrlAsync(string fileUrl);
        Task<bool> DeleteFileAsync(string fileId);
    }
}