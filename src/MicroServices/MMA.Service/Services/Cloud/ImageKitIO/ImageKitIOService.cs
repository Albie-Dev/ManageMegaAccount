using System.Text;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Http;
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

        public async Task<(int StatusCode, string JsonResponse)> GetFileByIdAsync(string fileId)
        {
            var fileResponse = await _imageKitClient.GetFileDetailAsync(fileId: fileId);
            return (StatusCode: fileResponse.HttpStatusCode, JsonResponse: fileResponse.Raw);
        }

        public async Task<(int StatusCode, string JsonResponse)> GetFileByUrlAsync(string fileUrl)
        {
            var fileResponse = GetFileDetail(fileId: fileUrl);
            return await Task.FromResult((StatusCode: StatusCodes.Status200OK, JsonResponse: fileResponse));
        }


        public string GetFileDetail(string fileId)
        {
            try
            {
                using var httpClient = new HttpClient();
                string privateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(RuntimeContext.AppSettings.CloudSetting.ImageKitIOConfig.PrivateKey + ":"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + privateKey);
                string requestUri = string.Format("https://api.imagekit.io/" + "v1/files/{0}/details", fileId);
                HttpResponseMessage result2 = httpClient.GetAsync(requestUri).Result;
                string result3 = result2.Content.ReadAsStringAsync().Result;
                
                // Utils.PopulateResponseMetadata(result3, result, Convert.ToInt32(result2.StatusCode), null);
                return result3;
            }
            catch (Exception ex)
            {
                throw new MMAException(statusCode: StatusCodes.Status500InternalServerError, errors: new List<ErrorDetailDto>(){ new ErrorDetailDto() { Error = ex.Message , ErrorScope = CErrorScope.PageSumarry } });
            }
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