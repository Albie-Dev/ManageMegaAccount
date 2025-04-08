using System.Collections;
using System.Security.Claims;
using Imagekit.Models;
using Imagekit.Sdk;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class ImageKitIOService : IImageKitIOService
    {
        private readonly ILogger<ImageKitIOService> _logger;
        private readonly ImagekitClient _imageKitClient;
        private readonly ITokenManager _tokenManager;

        public ImageKitIOService(
            ILogger<ImageKitIOService> logger,
            ImagekitClient imageKitClient,
            ITokenManager tokenManager
        )
        {
            _logger = logger;
            _imageKitClient = imageKitClient;
            _tokenManager = tokenManager;
        }


        public async Task<string> GenerateJWTTokenAsync(ImageKitIOGenerateJWTRequestDto requestDto)
        {
            var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = iat + Math.Min(requestDto.Expire, 3600);
            var payload = new Dictionary<string, object>
            {
                { "fileName", requestDto.UploadPayload.FileName },
                { "useUniqueFileName", requestDto.UploadPayload.UseUniqueFileName.ToString().ToLower() },
                { "tags", string.Join(",", requestDto.UploadPayload.Tags) },
                { "iat", iat },
                { "exp", exp }
            };

            var jwtToken = await _tokenManager.GenerateJWTTokenAsync(
                payload: payload,
                symmetricSecurityKey: RuntimeContext.AppSettings.CloudSetting.ImageKitIOConfig.PrivateKey,
                publicKey: requestDto.PublicKey
            );

            return await Task.FromResult(jwtToken);
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            var result = await _imageKitClient.DeleteFileAsync(fileId: fileId);
            return true;
        }
        #region base function

        #endregion base function
    }
}