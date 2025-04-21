
using Mapster;
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Movie.Actor.Update
{
    public partial class Update
    {
        [Parameter]
        public Guid ActorId { get; set; }
        private NotificationResponse? _notificationResponse { get; set; } = null;
        private UpdateActorRequestDto _requestDto { get; set; } = null!;
        private List<ImageKitIOFileResponseDto> UploadedFiles = new List<ImageKitIOFileResponseDto>();

        private bool _isLoading { get; set; } = false;
        public List<ErrorDetailDto> Errors { get; set; } = new();

        #region Initialized data from api detail
        protected override async Task OnInitializedAsync()
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.GetAsync<ActorDetailDto>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Detail, ActorId.ToString()),
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET
                );

                if (apiResponse != null)
                {
                    if (!apiResponse.Errors.IsNullOrEmpty())
                    {
                        Errors = apiResponse.Errors;
                    }
                    else if (apiResponse.Data == null)
                    {
                        _toastService.ShowError($"Không thể lấy thông tin diễn viên. ActorId = {ActorId}");
                    }
                    else
                    {
                        _requestDto = apiResponse.Data.Adapt<UpdateActorRequestDto>();
                        UploadedFiles = new List<ImageKitIOFileResponseDto>()
                        {
                            new ImageKitIOFileResponseDto()
                            {
                                FileId = _requestDto.FileId,
                                Name = string.Empty,
                                Url = _requestDto.Avatar
                            }
                        };
                    }
                }
                else
                {
                    _toastService.ShowError(message: $"Cannot connect to server. Host : {CPortalType.CET.ToDescription()}");
                }
            }
            catch(Exception ex)
            {
                _isLoading = false;
                _toastService.ShowError(ex.Message);
                await Task.Delay(delay: TimeSpan.FromSeconds(5));
                _navigationManager.NavigateTo("/movie/actor");
            }
            finally
            {
                _isLoading = false;
            }
        }
        #endregion Initialized data from api detail

        #region Update actor info
        public async Task UpdateActorAsync()
        {
            try
            {
                _isLoading = true;
                if (!UploadedFiles.IsNullOrEmpty())
                {
                    var fileDetail = UploadedFiles.First();
                    _requestDto.Avatar = fileDetail.Url;
                    _requestDto.FileId = fileDetail.FileId;
                }
                ResponseResult<NotificationResponse>? apiResponse = await _httpClientHelper.PostAsync<UpdateActorRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Update),
                    data: _requestDto,
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET);
                if (apiResponse == null)
                {
                    _toastService.ShowError(message: $"Không thể kết nối đến server. Host: {CPortalType.CET.ToDescription()}");
                }
                else if (!apiResponse.Errors.IsNullOrEmpty())
                {
                    Errors = apiResponse.Errors;
                }
                else if (apiResponse.Data == null)
                {
                    _toastService.ShowError(message: "Đã có lỗi xảy ra trong quá trình cập thông tin diễn viên.");
                }
                else
                {
                    _notificationResponse = apiResponse.Data;
                    _isLoading = false;
                    await Task.Delay(delay: TimeSpan.FromSeconds(value: 5));
                    _navigationManager.NavigateTo(uri: "/movie/actor", forceLoad: true);
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError(message: ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }
        #endregion Update actor info
    }
}