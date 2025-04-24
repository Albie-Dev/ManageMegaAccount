using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Movie.Actor.Create
{
    public partial class Create
    {
        private CreateActorRequestDto _requestDto { get; set; } = new CreateActorRequestDto();
        private NotificationResponse? _notificationResponse { get; set; }
        private bool _isLoading { get; set; } = false;
        private List<ImageKitIOFileResponseDto> UploadedFiles = new List<ImageKitIOFileResponseDto>();
        private List<ErrorDetailDto> Errors { get; set; } = new List<ErrorDetailDto>();

        protected override async Task OnInitializedAsync()
        {
            _requestDto.Status = CMasterStatus.Active;
            await Task.CompletedTask;
        }

        private async Task CreateActorAsync()
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
                ResponseResult<NotificationResponse>? apiResponse = await _httpClientHelper.PostAsync<CreateActorRequestDto, NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Create),
                    data: _requestDto,
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET);
                if (apiResponse == null)
                {
                    _toastService.ShowError(message: "An error occured while call to server.");
                }
                else if (!apiResponse.Errors.IsNullOrEmpty())
                {
                    Errors = apiResponse.Errors;
                }
                else if (apiResponse.Data == null)
                {
                    _toastService.ShowError(message: "Không thể nhận được phản hồi từ API.");
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
                _toastService.ShowError($"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}