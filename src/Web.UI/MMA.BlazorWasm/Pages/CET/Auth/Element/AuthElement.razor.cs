using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.Element
{
    public partial class AuthElement
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;

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
                    if (!apiResponse.Errors.IsNullOrEmpty())
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
                _newNotificationCount = 0;
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