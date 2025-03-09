using Blazored.LocalStorage;
using MMA.Domain;

namespace MMA.BlazorWasm
{
    public class AuthService : IAuthService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpClientHelper _httpClientHelper;
        private readonly ApiAuthenticationStateProvider _authenticationStateProvider;

        public AuthService(
            ILocalStorageService localStorage,
            IHttpClientHelper httpClientHelper,
            ApiAuthenticationStateProvider authenticationStateProvider)
        {
            _localStorage = localStorage;
            _httpClientHelper = httpClientHelper;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {

            var response = await _httpClientHelper.PostAsync<LoginRequestDto, LoginResponseDto>(
                endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_SystemLogin),
                data: new LoginRequestDto()
                {
                    Email = email,
                    Password = password
                }, requestType: CHttpClientType.Public, portalType: CPortalType.CET);
            if (response != null && response.Success && response.Data != null)
            {
                await _localStorage.SetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key, data: response.Data.ToJson());
                _authenticationStateProvider.MarkUserAsAuthenticated(tokenData: response.Data);
            }
            else
            {
                // need toast error here.
                return false;
            }
            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key);
            _authenticationStateProvider.MarkUserAsLoggedOut();
        }
    }
}