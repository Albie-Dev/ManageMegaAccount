using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Blazored.LocalStorage;
using MMA.Domain;

namespace MMA.BlazorWasm
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ApiAuthenticationStateProvider _authenticationStateProvider;
        public HttpClientHelper(IHttpClientFactory httpClient,
            ILocalStorageService localStorage,
            ApiAuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<HttpClient> GetHttpClientAsync(CHttpClientType httpClientType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var client = _httpClient.CreateClient(ApiClientConstant.HttpClient_Name);
            client.BaseAddress = new Uri(portalType.ToDescription());
            var jsonData = await _localStorage.GetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key) ?? string.Empty;
            var loginResponseDto = jsonData.FromJson<LoginResponseDto>();
            if (loginResponseDto == null)
            {
                return client;
            }
            if (httpClientType == CHttpClientType.Private)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: ApiClientConstant.API_Auth_Header_Scheme,
                    parameter: loginResponseDto.AccessToken);
            }
            else
            {
                client.DefaultRequestHeaders.Remove(name: ApiClientConstant.API_Header_Key);
            }
            return client;
        }

        public async Task<ResponseResult<TResponse>?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var httpClient = await GetHttpClientAsync(httpClientType: requestType, portalType: portalType);
            var jsonContent = data?.ToJson() ?? string.Empty;
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(requestUri: endpoint, content: content);
            // nếu kết quả trả về không xác thực được -> cần refresh access token mới.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ProcessRefreshTokenAsync(httpClient: httpClient, endpoint: endpoint,
                    requestType: CRequestType.Post, response: response, content: content);
            }
            var responseData = await response.Content.ReadAsStringAsync();
            var result = responseData.FromJson<ResponseResult<TResponse>>();
            return result;
        }

        public async Task<ResponseResult<TResponse>?> GetAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var httpClient = await GetHttpClientAsync(httpClientType: requestType, portalType: portalType);

            var response = await httpClient.GetAsync(requestUri: endpoint);
            // nếu kết quả trả về không xác thực được -> cần refresh access token mới.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ProcessRefreshTokenAsync(httpClient: httpClient, endpoint: endpoint,
                    response: response, content: null, requestType: CRequestType.Get);
            }
            var responseData = await response.Content.ReadAsStringAsync();
            var result = responseData.FromJson<ResponseResult<TResponse>>();
            return result;
        }

        public async Task<ResponseResult<TResponse>?> DeleteAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var httpClient = await GetHttpClientAsync(httpClientType: requestType, portalType: portalType);

            var response = await httpClient.DeleteAsync(requestUri: endpoint);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ProcessRefreshTokenAsync(httpClient: httpClient, endpoint: endpoint,
                    response: response, content: null, requestType: CRequestType.Delete);
            }

            var responseData = await response.Content.ReadAsStringAsync();
            var result = responseData.FromJson<ResponseResult<TResponse>>();
            return result;
        }


        private async Task ProcessRefreshTokenAsync(HttpClient httpClient, string endpoint,
            CRequestType requestType,
            HttpResponseMessage response, HttpContent? content = null)
        {
            var jsonData = await _localStorage.GetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key) ?? string.Empty;
            var loginResponseDto = jsonData.FromJson<LoginResponseDto>();
            var refreshAccessTokenResponse = await httpClient.PostAsync(requestUri: $"{EndpointConstant.CET_Base_Url}/{EndpointConstant.CET_Auth_RefreshToken}",
                content: new StringContent(content: (new RefreshAccessTokenRequestDto()
                {
                    AccessToken = loginResponseDto?.AccessToken ?? string.Empty,
                    RefreshToken = loginResponseDto?.RefreshToken ?? string.Empty
                }).ToJson(), encoding: Encoding.UTF8, mediaType: "application/json"));
            var userRefreshToken = (await refreshAccessTokenResponse.Content.ReadAsStringAsync())
                .FromJson<ResponseResult<LoginResponseDto>>();
            if (userRefreshToken.Data == null)
            {
                await _localStorage.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key);
                _authenticationStateProvider.MarkUserAsLoggedOut();
                return;
            }
            await _localStorage.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key);
            await _localStorage.SetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key, data: userRefreshToken.Data.ToJson());
            _authenticationStateProvider.MarkUserAsAuthenticated(tokenData: userRefreshToken.Data);
            if (userRefreshToken != null && userRefreshToken.Success)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: ApiClientConstant.API_Auth_Header_Scheme,
                    parameter: userRefreshToken.Data?.AccessToken);
                switch (requestType)
                {
                    case CRequestType.Get: {
                        response = await httpClient.GetAsync(requestUri: endpoint);
                        break;
                    }
                    case CRequestType.Post: {
                        response = await httpClient.PostAsync(requestUri: endpoint, content: content);
                        break;
                    }
                    case CRequestType.Put: {
                        response = await httpClient.PutAsync(requestUri: endpoint, content: content);
                        break;
                    }
                    case CRequestType.Delete: {
                        response = await httpClient.DeleteAsync(requestUri: endpoint);
                        break;
                    }
                    default: break;
                }
            }
            else
            {
                await _localStorage.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key);
                _authenticationStateProvider.MarkUserAsLoggedOut();
            }
        }
    }
}