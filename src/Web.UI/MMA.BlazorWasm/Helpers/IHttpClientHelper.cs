using Microsoft.AspNetCore.Components.Forms;
using MMA.Domain;

namespace MMA.BlazorWasm
{
    public interface IHttpClientHelper
    {
        Task<HttpResponseMessage?> BaseAPICallAsync<TRequest>(string endpoint,
            TRequest data,
            CRequestType methodType,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
        Task<ResponseResult<TResponse>?> GetAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
        Task<ResponseResult<TResponse>?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
        
        Task<ResponseResult<TResponse>?> PostAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);

        Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file, string endpoint, 
            CHttpClientType clientType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
        
        Task<ResponseResult<TResponse>?> DeleteAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
    }
}