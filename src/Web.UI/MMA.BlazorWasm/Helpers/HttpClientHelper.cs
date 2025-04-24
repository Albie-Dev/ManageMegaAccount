using System.Net;
using MMA.Domain;
using System.Text;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MMA.BlazorWasm
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly ApiAuthenticationStateProvider _authenticationStateProvider;
        public HttpClientHelper(IHttpClientFactory httpClient,
            ILocalStorageService localStorage,
            NavigationManager navigationManager,
            ApiAuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            _navigationManager = navigationManager;
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

        public async Task<HttpResponseMessage?> BaseAPICallAsync<TRequest>(string endpoint,
            TRequest? data,
            CRequestType methodType,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var httpClient = await GetHttpClientAsync(httpClientType: requestType, portalType: portalType);
            HttpResponseMessage? response = null;
            HttpContent? content = null;
            switch(methodType)
            {
                case CRequestType.Get:{
                    response = await httpClient.GetAsync(requestUri: endpoint);
                    break;
                }
                case CRequestType.Post:{
                    var jsonContent = data?.ToJson() ?? string.Empty;
                    content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await httpClient.PostAsync(requestUri: endpoint, content: data == null ? null : content);
                    break;
                }
                case CRequestType.Put:{

                    break;
                }
                case CRequestType.Delete:{

                    break;
                }
                default: break;
            }

            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ProcessRefreshTokenAsync(httpClient: httpClient, endpoint: endpoint,
                    requestType: methodType, response: response, content: content);
            }

            await ProcessAPIStatusResponseAsync(response?.StatusCode ?? 0);
            return response;
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

            await ProcessAPIStatusResponseAsync(response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();
            var result = responseData.FromJson<ResponseResult<TResponse>>();
            return result;
        }

        public async Task<ResponseResult<TResponse>?> PostAsync<TResponse>(
            string endpoint,
            CHttpClientType requestType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var httpClient = await GetHttpClientAsync(httpClientType: requestType, portalType: portalType);

            var response = await httpClient.PostAsync(requestUri: endpoint, content: null);
            // nếu kết quả trả về không xác thực được -> cần refresh access token mới.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ProcessRefreshTokenAsync(httpClient: httpClient, endpoint: endpoint,
                    requestType: CRequestType.Post, response: response, content: null);
            }

            await ProcessAPIStatusResponseAsync(response.StatusCode);

            var responseData = await response.Content.ReadAsStringAsync();
            var result = responseData.FromJson<ResponseResult<TResponse>>();
            return result;
        }


        public async Task<HttpResponseMessage> UploadFileAsync(IBrowserFile file, string endpoint, 
            CHttpClientType clientType = CHttpClientType.Private,
            CPortalType portalType = CPortalType.CET)
        {
            var httpClient = await GetHttpClientAsync(clientType, portalType);

            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(15 * 1024 * 1024));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.Name);

            var response = await httpClient.PostAsync(endpoint, content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await ProcessRefreshTokenAsync(httpClient, endpoint, CRequestType.Post, response, content);
            }

            await ProcessAPIStatusResponseAsync(response.StatusCode);

            return response;
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

            await ProcessAPIStatusResponseAsync(response.StatusCode);

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

            await ProcessAPIStatusResponseAsync(response.StatusCode);

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

        private async Task ProcessAPIStatusResponseAsync(HttpStatusCode statusCode)
        {
            switch(statusCode)
            {
                case HttpStatusCode.Forbidden:
                {
                    _navigationManager.NavigateTo(uri: "/forbiden/403");
                    break;
                }
                case HttpStatusCode.Unauthorized:
                {
                    _navigationManager.NavigateTo(uri: "/login");
                    break;
                }
            }
            await Task.CompletedTask;
        }
    }
}