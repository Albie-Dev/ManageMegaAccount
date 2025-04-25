using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Auth.Login
{
    public partial class LoginPage
    {
        [Inject] private ILocalStorageService _localStorageService { get; set; } = default!;
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        private List<ErrorDetailDto> Errors { get; set; } = new();
        private LoginRequestDto LoginRequestDto = new();
        private bool IsLoading { get; set; } = false;
        private async Task MicrosoftGenerateLoginUrlAsyc()
        {
            try
            {
                IsLoading = true;
                string state = Guid.NewGuid().ToString();
                var urlResponse = await _httpClientHelper.PostAsync<GenerateMicrosoftLoginUrlRequestDto, RedirectResponseDto>(
                    endpoint: Path.Combine(path1: EndpointConstant.CET_Base_Url, path2:
                    EndpointConstant.CET_Auth_Microsoft_Generate_Login_Url),
                    data: new GenerateMicrosoftLoginUrlRequestDto()
                    {
                        State = state
                    },
                    requestType: CHttpClientType.Public,
                    portalType: CPortalType.CET);

                if (urlResponse == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!urlResponse.Errors.IsNullOrEmpty())
                    {
                        Errors = urlResponse.Errors;
                    }
                    else if (urlResponse.Data == null)
                    {
                        _toastService.ShowError("An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        await _localStorageService.SetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key_Microsoft_State,
                        data: state);
                        IsLoading = false;
                        _navigationManager.NavigateTo(uri: urlResponse.Data.Url);
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

        private async Task HandleLogin()
        {
            try
            {
                IsLoading = true;
                Errors.Clear();
                var response = await _httpClientHelper.PostAsync<LoginRequestDto, LoginResponseDto>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_SystemLogin),
                    data: LoginRequestDto, requestType: CHttpClientType.Public, portalType: CPortalType.CET);

                if (response == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!response.Errors.IsNullOrEmpty())
                    {
                        Errors = response.Errors;
                    }
                    else if (response.Data == null)
                    {
                        _toastService.ShowError($"An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        if (response.Data.TwoFactorEnable)
                        {
                            _navigationManager.NavigateTo(uri: $"/twofactorauthentication?email={Uri.EscapeDataString(LoginRequestDto.Email)}&token={Uri.EscapeDataString(response.Data.TokenUrlSecret)}", forceLoad: true);
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

        private async Task GoogleGenerateLoginUrlAsyc()
        {
            try
            {
                IsLoading = true;
                string state = Guid.NewGuid().ToString();

                var urlResponse = await _httpClientHelper.PostAsync<GenerateGoogleLoginUrlRequestDto, RedirectResponseDto>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Auth_Google_Generate_Login_Url),
                    data: new GenerateGoogleLoginUrlRequestDto { State = state },
                    requestType: CHttpClientType.Public,
                    portalType: CPortalType.CET);

                if (urlResponse == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!urlResponse.Errors.IsNullOrEmpty())
                    {
                        Errors = urlResponse.Errors;
                    }
                    else if (urlResponse.Data == null)
                    {
                        _toastService.ShowError($"An error occured while fetch data. Server no response data.");
                    }
                    else
                    {
                        await _localStorageService.SetItemAsStringAsync(ApiClientConstant.LocalStorage_Key_Google_State, state);
                        _navigationManager.NavigateTo(urlResponse.Data.Url);
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
    }
}