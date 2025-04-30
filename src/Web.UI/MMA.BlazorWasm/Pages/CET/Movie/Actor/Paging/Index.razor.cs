using Microsoft.JSInterop;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Movie.Actor.Paging
{
    public partial class Index
    {

        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private ActorFilterProperty _actorFilterProperty { get; set; } = new ActorFilterProperty();
        private List<Guid> _selectedActorIds { get; set; } = new List<Guid>();
        private bool _isLoading { get; set; } = false;
        private NotificationResponse? _notificationResponse { get; set; }


        #region variable
        private bool _isShowImage { get; set; } = false;

        private TableParam<ActorFilterProperty> _requestDto { get; set; } = new TableParam<ActorFilterProperty>();
        #endregion variable

        #region action
        private bool ShowUpdateActor => _selectedActorIds.Count == 1;
        private bool ShowDeactiveActor => _selectedActorIds.Count == 0;
        #region view detail

        #endregion view detail

        #region view image
        private string currentImageUrl = string.Empty;

        private void OpenImageViewer(string imageUrl)
        {
            currentImageUrl = imageUrl;
            _isShowImage = true;
        }

        private void CloseImageViewer()
        {
            _isShowImage = false;
        }
        #endregion view image

        #region update
        private async Task UpdateActorAsync()
        {
            if (!_selectedActorIds.IsNullOrEmpty() && _selectedActorIds.Count == 1)
            {
                _navigationManager.NavigateTo($"/movie/actor/update/{_selectedActorIds.First()}");
            }
            await Task.CompletedTask;
        }
        #endregion update

        #region delete
        private async Task DeleteActorsAsync()
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.PostAsync<DeleteActorRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Delete),
                    data: new DeleteActorRequestDto()
                    {
                        ActorId = _selectedActorIds.First()
                    },
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET
                );
                if (apiResponse == null)
                {
                    _toastService.ShowError($"Không thể kết nối đến server. Host: {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!apiResponse.Errors.IsNullOrEmpty())
                    {
                        _errors = apiResponse.Errors;
                    }
                    else if (apiResponse.Data == null)
                    {
                        _toastService.ShowError($"Đã có lỗi xảy ra trong quá trình xóa diễn viên.");
                    }
                    else
                    {
                        _notificationResponse = apiResponse.Data;
                    }
                }
            }
            catch(Exception ex)
            {
                _toastService.ShowError(ex.Message);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }
        #endregion delete

        #region deactive
        private async Task DeactiveActorsAsync()
        {
            if (!_selectedActorIds.IsNullOrEmpty())
            {
                try
                {
                    _isLoading = true;
                    var apiResponse = await _httpClientHelper.PostAsync<DeactiveActorRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Deactive),
                    data: new DeactiveActorRequestDto()
                    {
                        ActorIds = _selectedActorIds
                    },
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET);
                    if (apiResponse != null)
                    {
                        if (!apiResponse.Errors.IsNullOrEmpty())
                        {
                            _errors = apiResponse.Errors;
                        }
                        else if (apiResponse.Data == null)
                        {
                            _toastService.ShowError($"Đã có lỗi xảy ra trong quá trình deactive actors.");
                        }
                        else
                        {
                            _notificationResponse = apiResponse.Data;
                        }
                    }
                    else
                    {
                        _toastService.ShowError($"Không thể kết nối đến server. Host : {CPortalType.CET.ToDescription()}");
                    }
                }
                catch (Exception ex)
                {
                    _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
                }
                finally
                {
                    _isLoading = false;
                    StateHasChanged();
                }
            }
        }
        #endregion deactive

        #region create
        private async Task CreateActorAsync()
        {
            await Task.CompletedTask;
            _navigationManager.NavigateTo(uri: "/movie/actor/create");
        }
        #endregion create

        #region import
        private bool _isUploadModalOpen { get; set; } = false;
        public void HandleOpenUploadModal()
        {
            _isUploadModalOpen = true;
        }

        public async Task DownloadTemplateFileAsync()
        {
            try
            {
                var apiResponse = await _httpClientHelper.BaseAPICallAsync<TableParam<ActorFilterProperty>>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Template),
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
            catch(Exception ex)
            {
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isUploadModalOpen = false;
                _isLoading = false;
            }
        }

        public async Task ImportActorsAsync()
        {
            await Task.CompletedTask;
        }
        #endregion import

        #region export
        private async Task ExportActorsAsync()
        {
            try
            {
                _isLoading = true;
                var filter = _requestDto.Filter;
                if (filter == null)
                {
                    filter = new ActorFilterProperty();
                }
                filter.ActorIds = _selectedActorIds;
                var apiResponse = await _httpClientHelper.BaseAPICallAsync<TableParam<ActorFilterProperty>>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Export),
                    data: new TableParam<ActorFilterProperty>()
                    {
                        Filter = filter,
                        PageNumber = 1,
                        PageSize = CoreConstant.MAX_EXPORT_ITEMS,
                        SearchQuery = _requestDto.SearchQuery,
                        Sorter = _requestDto.Sorter
                    },
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
            catch(Exception ex)
            {
                _toastService.ShowError(message: ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }
        #endregion export

        #endregion action
    }
}