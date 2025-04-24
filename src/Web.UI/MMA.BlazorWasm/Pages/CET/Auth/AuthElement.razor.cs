using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth
{
    public partial class AuthElement
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        private bool IsLoad { get; set; } = false;
        private async Task LogoutAsync()
        {
            try
            {
                IsLoad = true;
                var loginResponseDto = (await _localStorageService.GetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key) ?? string.Empty)
                    .FromJson<LoginResponseDto>();
                if (loginResponseDto != null)
                {
                    var response = await _httpClientHelper.PostAsync<LogoutRequestDto, RedirectResponseDto>(
                        endpoint: Path.Combine(path1: EndpointConstant.CET_Base_Url, path2: EndpointConstant.CET_Auth_Logout),
                        data: new LogoutRequestDto()
                        {
                            IsAllDevice = false,
                            AuthType = loginResponseDto.AuthType
                        },
                        requestType: CHttpClientType.Private,
                        portalType: CPortalType.CET);
                    await _localStorageService.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key);
                    await _localStorageService.RemoveItemAsync(key: ApiClientConstant.LocalStorage_Key_Microsoft_State);
                    _authProvider.MarkUserAsLoggedOut();
                    StateHasChanged();
                    IsLoad = false;
                    if (response != null && response.Success
                        && response.Data != null
                        && !string.IsNullOrEmpty(response.Data.Url))
                    {
                        _navigationManager.NavigateTo(uri: response.Data.Url);
                    }
                    else
                    {
                        _navigationManager.NavigateTo("/", forceLoad: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                IsLoad = false;
            }
        }
    }
}