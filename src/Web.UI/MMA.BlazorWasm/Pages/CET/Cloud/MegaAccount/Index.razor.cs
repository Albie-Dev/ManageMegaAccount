
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Cloud.MegaAccount
{
    public partial class Index
    {
        private bool _isLoading { get; set; } = false;
        private bool _isUploadModalOpen { get; set; } = false;
        private NotificationResponse? _notificationResponse { get; set; }
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private MegaAccountFilterProperty _megaAccountFilterProperty { get; set; } = new MegaAccountFilterProperty();
        private List<Guid> _megaAccountIds { get; set; } = new List<Guid>();
        private TableParam<MegaAccountFilterProperty> _requestDto { get; set; } = new TableParam<MegaAccountFilterProperty>();

        private bool ShowUpdateMegaAccount => _megaAccountIds.Count == 1;
        private bool ShowDeactiveMegaAccount => _megaAccountIds.Count > 0;

        private async Task ImportMegaAccountsAsync(IBrowserFile importFile)
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.UploadFileAsync(file: importFile, endpoint: Path.Combine(EndpointConstant.Cloud_Base_Url,
                EndpointConstant.Cloud_Mega_Account_Template),
                clientType: CHttpClientType.Private,
                portalType: CPortalType.CET);
                if (apiResponse == null)
                {
                    _toastService.ShowError($"Không thể kết nối đến server. Host: {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        string exportFileName = apiResponse.Content.Headers.ContentDisposition?.FileName ?? string.Empty;
                        byte[] fileBytes = await apiResponse.Content.ReadAsByteArrayAsync();
                        var base64 = Convert.ToBase64String(fileBytes);
                        await _jsRuntime.InvokeVoidAsync("downloadFile", exportFileName, base64);
                        HandleOpenUploadModal();
                    }
                    else
                    {
                        _toastService.ShowError(message: $"{await apiResponse.Content.ReadAsStringAsync()}");
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

        private async Task ExportMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

        private void HandleOpenUploadModal()
        {
            _isUploadModalOpen = !_isUploadModalOpen;
        }

        private async Task DownloadTemplateFileAsync()
        {
            try
            {
                var apiResponse = await _httpClientHelper.BaseAPICallAsync<TableParam<ActorFilterProperty>>(
                    endpoint: Path.Combine(EndpointConstant.Cloud_Base_Url, EndpointConstant.Cloud_Mega_Account_Template),
                    data: null,
                    methodType: CRequestType.Post,
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET);
                if (apiResponse == null)
                {
                    _toastService.ShowError($"Không thể kết nối đến server. Host: {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        string exportFileName = apiResponse.Content.Headers.ContentDisposition?.FileName ?? string.Empty;
                        byte[] fileBytes = await apiResponse.Content.ReadAsByteArrayAsync();
                        var base64 = Convert.ToBase64String(fileBytes);
                        await _jsRuntime.InvokeVoidAsync("downloadFile", exportFileName, base64);
                    }
                    else
                    {
                        _toastService.ShowError(message: $"");
                    }
                }

            }
            catch (Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isUploadModalOpen = false;
                _isLoading = false;
            }
        }

        public async Task CreateMegaAccountAsync()
        {
            await Task.CompletedTask;
        }

        private async Task UpdateMegaAccountAsync()
        {
            await Task.CompletedTask;
        }

        private async Task DeactiveMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

        private async Task DeleteMegaAccountsAsync()
        {
            await Task.CompletedTask;
        }

        private async Task MegaAccountLoginAsync(Guid megaAccountId)
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.PostAsync<MegaAccountLoginRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.Cloud_Base_Url, EndpointConstant.Cloud_Mega_Account_Login),
                    data: new MegaAccountLoginRequestDto() { MegaAccountId = megaAccountId },
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
                        _notificationResponse = apiResponse.Data;
                    }
                }
            }
            catch(Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task MegaAccountSyncDataAsync(Guid megaAccountId)
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.PostAsync<MegaAccountLoginRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.Cloud_Base_Url, EndpointConstant.Cloud_Mega_Account_Sync),
                    data: new MegaAccountLoginRequestDto() { MegaAccountId = megaAccountId },
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
                        _notificationResponse = apiResponse.Data;
                    }
                }
            }
            catch(Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isLoading = false;
            }
        }

    }
}