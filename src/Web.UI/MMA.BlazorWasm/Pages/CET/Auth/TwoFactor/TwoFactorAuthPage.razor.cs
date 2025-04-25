using System.Web;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.TwoFactor
{
    public partial class TwoFactorAuthPage
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        private bool _isLoading { get; set; } = false;
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private TwoFactorModel _twoFactorModel { get; set; } = new TwoFactorModel();

        private string _email { get; set; } = string.Empty;
        private string _token { get; set; } = string.Empty;

        private ConfirmTwoFactorAuthenticationRequestDto _requestDto = new ConfirmTwoFactorAuthenticationRequestDto();


        protected override async Task OnInitializedAsync()
        {
            try
            {
                _isLoading = true;
                var uri = new Uri(_navigationManager.Uri);
                _token = HttpUtility.ParseQueryString(uri.Query).Get("token") ?? string.Empty;
                _email = HttpUtility.ParseQueryString(uri.Query).Get("email") ?? string.Empty;
                _requestDto.Email = _email;
                _requestDto.TokenUrlSecret = _token;
                if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_email))
                {
                    _toastService.ShowError("Invalid two factor authentication secret token. You request access to denied.");
                    _navigationManager.NavigateTo("/");
                }
                else
                {
                    // call to server check token
                    var apiResonse = await _httpClientHelper.PostAsync<TwoFactorVerifyTokenRequestDto, bool>(
                        endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_TwoFactor_VerifyToken),
                        data: new TwoFactorVerifyTokenRequestDto()
                        {
                            Token = _token
                        },
                        requestType: CHttpClientType.Private,
                        portalType: CPortalType.CET);
                    if (apiResonse == null)
                    {
                        _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                    }
                    else
                    {
                        if (!apiResonse.Errors.IsNullOrEmpty())
                        {
                            _errors = apiResonse.Errors;
                        }
                        else if (!apiResonse.Data)
                        {
                            _toastService.ShowError("Invalid two factor authentication secret token. You request access to denied.");
                            _navigationManager.NavigateTo("/");
                        }
                        else
                        {
                            // valid token
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
                _navigationManager.NavigateTo("/");

            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task ConfirmTwoFactorAuthenticationAsync()
        {
            try
            {
                _isLoading = true;
                _requestDto.Token = _twoFactorModel.GetFullCode();
                var apiResponse = await _httpClientHelper.PostAsync<ConfirmTwoFactorAuthenticationRequestDto, LoginResponseDto>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_TwoFactor_Confirm),
                    data: _requestDto,
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
                        _toastService.ShowError($"An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        await _localStorageService.SetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key, data: apiResponse.Data.ToJson());
                        _authProvider.MarkUserAsAuthenticated(tokenData: apiResponse.Data);
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
                _isLoading = false;
            }
        }

        private async Task HandlePaste(ClipboardEventArgs e)
        {
            var clipboardText = await GetClipboardTextAsync();
            if (clipboardText.Length == 6)
            {
                _twoFactorModel.Digit1 = clipboardText[0].ToString();
                _twoFactorModel.Digit2 = clipboardText[1].ToString();
                _twoFactorModel.Digit3 = clipboardText[2].ToString();
                _twoFactorModel.Digit4 = clipboardText[3].ToString();
                _twoFactorModel.Digit5 = clipboardText[4].ToString();
                _twoFactorModel.Digit6 = clipboardText[5].ToString();
            }
        }
        private async Task<string> GetClipboardTextAsync()
        {
            var clipboardText = await _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
            return clipboardText;
        }

        private async Task MoveToNext(ChangeEventArgs e, string nextElementId)
        {
            if (!string.IsNullOrEmpty(e.Value?.ToString()))
            {
                await _jsRuntime.InvokeVoidAsync("moveToNext", nextElementId);
            }
        }
    }
}