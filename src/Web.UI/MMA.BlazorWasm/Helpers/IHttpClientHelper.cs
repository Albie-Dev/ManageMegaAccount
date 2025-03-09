using MMA.Domain;

namespace MMA.BlazorWasm
{
    public interface IHttpClientHelper
    {
        Task<ResponseResult<TResponse>?> GetAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
        Task<ResponseResult<TResponse>?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET);
    }
}