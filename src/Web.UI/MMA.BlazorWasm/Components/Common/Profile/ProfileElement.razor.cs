using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Profile
{
    public partial class ProfileElement
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        [Parameter]
        public UserBaseInfoDto UserInfo { get; set; } = new UserBaseInfoDto();


        private bool _isLoading { get; set; } = false;


        private async Task LogoutAsync(bool isAllDevice = false)
        {
            try
            {
                _isLoading = true;
                var loginResponseDto = (await _localStorageService.GetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key) ?? string.Empty)
                    .FromJson<LoginResponseDto>();
                if (!string.IsNullOrEmpty(loginResponseDto.AccessToken))
                {
                    var response = await _httpClientHelper.PostAsync<LogoutRequestDto, RedirectResponseDto>(
                        endpoint: Path.Combine(path1: EndpointConstant.CET_Base_Url, path2: EndpointConstant.CET_Auth_Logout),
                        data: new LogoutRequestDto()
                        {
                            IsAllDevice = isAllDevice,
                            AuthType = loginResponseDto.AuthType
                        },
                        requestType: CHttpClientType.Private,
                        portalType: CPortalType.CET);
                    await _localStorageService.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key);
                    await _localStorageService.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key_Microsoft_State);
                    _authProvider.MarkUserAsLoggedOut();
                    StateHasChanged();
                    _isLoading = false;
                    if (response != null && response.Success
                        && response.Data != null
                        && !string.IsNullOrEmpty(response.Data.Url))
                    {
                        _navigationManager.NavigateTo(uri: response.Data.Url);
                    }
                    else
                    {
                        _navigationManager.NavigateTo("/login", forceLoad: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private string GetInitial(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var initial = names[0][0].ToString();
            if (names.Length > 1)
            {
                initial += names[^1][0];
            }
            return initial.ToUpper();
        }
    }
}