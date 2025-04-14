using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Movie.Actor
{
    public partial class Create
    {
        private CreateActorRequestDto _requestDto { get; set; } = new CreateActorRequestDto();
        private NotificationResponse? _notificationResponse { get; set; }
        private bool _isLoading { get; set; } = false;
        List<DropdownItemModel> RegionItems { get; set; } = new List<DropdownItemModel>();
        List<DropdownItemModel> CupSizeItems { get; set; } = new List<DropdownItemModel>();

        private List<ImageKitIOFileResponseDto> UploadedFiles = new List<ImageKitIOFileResponseDto>();

        private DropdownItemModel? RegionDropItem { get; set; }
        private DropdownItemModel? CupSizeDropItem { get; set; }

        private List<ErrorDetailDto> Errors { get; set; } = new List<ErrorDetailDto>();

        protected override async Task OnInitializedAsync()
        {
            CRegionType regionType = CRegionType.None;
            RegionItems = regionType.ToDropdownModels();
            RegionDropItem = new DropdownItemModel()
            {
                Name = RegionItems.Where(s => s.Value.ToString() == regionType.ToString()).Select(s => s.Name).FirstOrDefault() ?? string.Empty,
                Value = regionType
            };

            CCupSizeType cupSizeType = CCupSizeType.None;
            CupSizeItems = cupSizeType.ToDropdownModels();
            CupSizeDropItem = new DropdownItemModel()
            {
                Name = CupSizeItems.Where(s => s.Value.ToString() == cupSizeType.ToString()).Select(s => s.Name).FirstOrDefault() ?? string.Empty,
                Value = cupSizeType
            };

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
                }
                _isLoading = false;
                await Task.Delay(delay: TimeSpan.FromSeconds(value: 5));
                _navigationManager.NavigateTo(uri: "/movie/actor", forceLoad: true);

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

        private void OnRegionSelected(DropdownItemModel option)
        {
            RegionDropItem = option;
            _requestDto.RegionType = Enum.Parse<CRegionType>(value: option.Value.ToString() ?? string.Empty);
        }

        private void OnCupSizeSelected(DropdownItemModel option)
        {
            CupSizeDropItem = option;
            _requestDto.CupSizeType = Enum.Parse<CCupSizeType>(value: option.Value.ToString() ?? string.Empty);
        }
    }
}