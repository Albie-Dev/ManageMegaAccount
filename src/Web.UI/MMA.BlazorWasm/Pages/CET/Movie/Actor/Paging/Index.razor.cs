using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Movie.Actor.Paging
{
    public partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            _displayColumns = InitDisplayColumns();
            await FetchDataAsync();
        }

        private async Task FetchDataAsync()
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.PostAsync<TableParam<ActorFilterProperty>, BasePagedResult<ActorDetailDto>>(
                endpoint: Path.Combine(EndpointConstant.Movie_Base_Url, EndpointConstant.Movie_Actor_Paging),
                data: _requestDto, requestType: CHttpClientType.Private, portalType: CPortalType.CET);
                if (apiResponse == null)
                {
                    _toastService.ShowError($"An error occured while call to get actors.");
                }
                else if (!apiResponse.Errors.IsNullOrEmpty())
                {
                    _errors = apiResponse.Errors;
                }
                else if (apiResponse.Data == null)
                {
                    _toastService.ShowError(message: "không thể get data");
                }
                else
                {
                    _result = apiResponse.Data;
                }

            }
            catch (Exception ex)
            {
                _toastService.ShowError(ex.Message);
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }


        #region variable
        private bool _isShowImage { get; set; } = false;
        private bool _isLoading { get; set; } = false;
        private bool _isShowColumns { get; set; } = false;


        private TableParam<ActorFilterProperty> _requestDto { get; set; } = new TableParam<ActorFilterProperty>();
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private NotificationResponse? _notificationResponse { get; set; } = null;
        private BasePagedResult<ActorDetailDto> _result = new BasePagedResult<ActorDetailDto>();
        private List<DropdownItemModel> _displayColumns { get; set; } = new List<DropdownItemModel>();
        #endregion variable


        #region event

        #region filter

        #endregion filter

        #region sorter
        private async Task SortAsync(string propertyName)
        {
            if (_requestDto.Sorter == null)
            {
                _requestDto.Sorter = new();
            }
            _requestDto.Sorter.KeyName = propertyName;
            _requestDto.Sorter.IsASC = !_requestDto.Sorter.IsASC;
            await FetchDataAsync();
        }
        #endregion sorter

        #region paging

        #endregion paging

        #region search

        #endregion search

        #region column selection
        private void ApplyColumnSelection(List<DropdownItemModel> selectedColumns)
        {
            foreach (var column in _displayColumns)
            {
                column.IsSelected = selectedColumns.Any(c => c.Name == column.Name && c.IsSelected);
            }

        }
        private void CancelColumnSelection()
        {
            _isShowColumns = false;
        }
        #endregion column selection

        #endregion event


        #region action
        private List<Guid> _selectedActorIds { get; set; } = new List<Guid>();

        private bool ShowUpdateActor => _selectedActorIds.Count == 1;
        private bool ShowDeactiveActor => _selectedActorIds.Count == 0;
        public void SetSelectedActorId(Guid actorId)
        {
            if (_selectedActorIds.Contains(actorId))
            {
                _selectedActorIds.Remove(item: actorId);
            }
            else
            {
                _selectedActorIds.Add(item: actorId);
            }
        }
        #region view detail

        #endregion view detail

        #region view image
        private string currentImageUrl = "";

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
                            await FetchDataAsync();
                        }
                    }
                    else
                    {
                        _toastService.ShowError($"Không thể kết nối đến server. Host : {CPortalType.CET.ToDescription()}");
                    }
                }
                catch (Exception ex)
                {
                    _toastService.ShowError(ex.Message);
                }
                finally
                {
                    _isLoading = false;
                }
            }
        }
        #endregion deactive

        #region create
        private async Task CreateActorAsync()
        {
            await Task.CompletedTask;
            _navigationManager.NavigateTo(uri: "/movie/actor/create", forceLoad: true);
        }
        #endregion create

        #region import

        #endregion import

        #region export

        #endregion export

        #endregion action
        private List<DropdownItemModel> InitDisplayColumns()
        {
            return new List<DropdownItemModel>()
            {
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Name)}_Entry"),
                    Value = nameof(ActorDetailDto.Name),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.DateOfBirth)}_Entry"),
                    Value = nameof(ActorDetailDto.DateOfBirth),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.DebutDate)}_Entry"),
                    Value = nameof(ActorDetailDto.DebutDate),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Avatar)}_Entry"),
                    Value = nameof(ActorDetailDto.Avatar),
                    IsSelected = true,
                    IsSort = false
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Height)}_Entry"),
                    Value = nameof(ActorDetailDto.Height),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Waist)}_Entry"),
                    Value = nameof(ActorDetailDto.Waist),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Bust)}_Entry"),
                    Value = nameof(ActorDetailDto.Bust),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Hips)}_Entry"),
                    Value = nameof(ActorDetailDto.Hips),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.CupSizeType)}_Entry"),
                    Value = nameof(ActorDetailDto.CupSizeType),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.RegionType)}_Entry"),
                    Value = nameof(ActorDetailDto.RegionType),
                    IsSelected = true,
                    IsSort = true
                },
                new DropdownItemModel()
                {
                    Name = I18NHelper.GetString(key: $"Column_Title_Actor_{nameof(ActorDetailDto.Status)}_Entry"),
                    Value = nameof(ActorDetailDto.Status),
                    IsSelected = true,
                    IsSort = true
                }
            };
        }
    }
}