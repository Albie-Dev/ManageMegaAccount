using Imagekit.Models;
using MMA.Domain;

namespace MMA.Service
{
    public interface IImageKitIOService
    {
        Task<string> GenerateJWTTokenAsync(ImageKitIOGenerateJWTRequestDto requestDto);
        Task<bool> DeleteFileAsync(string fileId);
    }
}