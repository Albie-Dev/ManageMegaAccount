using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.Element
{
    public partial class AuthElement
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();

        private UserBaseInfoDto _userInfo { get; set; } = new UserBaseInfoDto();
        private int _newNotificationCount { get; set; } = 0;

        private bool IsLoad { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            var loginResponseDto = (await _localStorageService.GetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key) ?? string.Empty)
                .FromJson<LoginResponseDto>();
            if (string.IsNullOrEmpty(loginResponseDto.AccessToken))
            {
                return;
            }

            try
            {
                _newNotificationCount = await CountNewNotificationAsync();

                var apiResponse = await _httpClientHelper.GetAsync<UserBaseInfoDto>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_User_BaseInfo),
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET);

                if (apiResponse == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (apiResponse.Errors.IsNullOrEmpty())
                    {
                        _errors = apiResponse.Errors;
                    }
                    else if (apiResponse.Data == null)
                    {
                        _toastService.ShowError(message: "An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        _userInfo = apiResponse.Data;
                    }
                }
            }
            catch(Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                IsLoad = false;
            }
        }

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

        private async Task<int> CountNewNotificationAsync()
        {
            var apiResponse = await _httpClientHelper.GetAsync<int>(
                endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Notification_CountNew),
                requestType: CHttpClientType.Private,
                portalType: CPortalType.CET);
            if (apiResponse == null)
            {
                _toastService.ShowError(message: $"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                return 0;
            }
            else
            {
                if (!apiResponse.Errors.IsNullOrEmpty())
                {
                    _errors = apiResponse.Errors;
                    return 0;
                }
                else
                {
                    return apiResponse.Data;
                }
            }
        }
    }
}