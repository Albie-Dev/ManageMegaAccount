using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.Login
{
    public partial class LoginCallBack
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;

        private string IdToken { get; set; } = string.Empty;
        private string State { get; set; } = string.Empty;
        private bool IsLoading { get; set; }

        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private NotificationResponse? NotificationResponse { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            var url = _navigationManager.Uri.ToString();
            var currentState = await _localStorageService.GetItemAsStringAsync(ApiClientConstant.LocalStorage_Key_Microsoft_State);

            if (url.Contains('#'))
            {
                var queryParams = System.Web.HttpUtility.ParseQueryString(url.Split('#')[1]);
                IdToken = queryParams["id_token"] ?? string.Empty;
                State = queryParams["state"] ?? string.Empty;

                if (!string.Equals(State, currentState, StringComparison.OrdinalIgnoreCase))
                {
                    ShowErrorAndRedirect("Thông tin xác thực không hợp lệ.");
                }
                else
                {
                    await HandleLogin();
                }
            }
            else
            {
                ShowError("Thông tin xác thực không hợp lệ.");
            }
        }

        private async Task HandleLogin()
        {
            try
            {
                var response = await _httpClientHelper.PostAsync<LoginWithMicrosoftRequestDto, LoginResponseDto>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_Microsoft_Login),
                    data: new LoginWithMicrosoftRequestDto { IdToken = IdToken, State = string.Empty },
                    requestType: CHttpClientType.Public, portalType: CPortalType.CET);

                if (response == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!response.Errors.IsNullOrEmpty())
                    {
                        _errors = response.Errors;
                    }
                    else if (response.Data == null)
                    {
                        _toastService.ShowError($"An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        await _localStorageService.SetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key, data: response.Data.ToJson());
                        _authProvider.MarkUserAsAuthenticated(tokenData: response.Data);
                        StateHasChanged();
                        _navigationManager.NavigateTo(uri: "/", forceLoad: true);
                    }
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowError(string message)
        {
            NotificationResponse = new NotificationResponse()
            {
                DisplayType = CNotificationDisplayType.Redirect,
                Level = CNotificationLevel.Error,
                Message = message,
                Type = CNotificationType.Auth
            };
            _toastService.ShowError(message);
        }

        private void ShowErrorAndRedirect(string message)
        {
            ShowError(message);
            IsLoading = false;
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }
    }
}