using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.Login
{
    public partial class GoogleLoginCallback
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        private string Code { get; set; } = string.Empty;
        private string State { get; set; } = string.Empty;
        private bool IsLoading { get; set; }

        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();

        private NotificationResponse? NotificationResponse { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            var url = new Uri(_navigationManager.Uri);
            var queryParams = System.Web.HttpUtility.ParseQueryString(url.Query);
            Code = queryParams["code"] ?? string.Empty;
            State = queryParams["state"] ?? string.Empty;
            var currentState = await _localStorageService.GetItemAsStringAsync(ApiClientConstant.LocalStorage_Key_Google_State);

            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(State) || !string.Equals(State, currentState,
            StringComparison.OrdinalIgnoreCase))
            {
                ShowErrorAndRedirect("Thông tin xác thực không hợp lệ.");
                return;
            }

            await HandleLogin();
        }


        private async Task HandleLogin()
        {
            try
            {
                var response = await _httpClientHelper.PostAsync<LoginWithGoogleRequestDto, LoginResponseDto>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_Google_Login),
                    data: new LoginWithGoogleRequestDto { Code = Code, State = string.Empty },
                    requestType: CHttpClientType.Public, portalType: CPortalType.CET);

                if (response == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (response.Errors.IsNullOrEmpty())
                    {
                        _errors = response.Errors;
                    }
                    else if (response.Data == null)
                    {
                        _toastService.ShowError($"An error occured while fetch data from server no response data.");
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