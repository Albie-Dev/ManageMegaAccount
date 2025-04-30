using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Table
{
    public partial class Table<TData, TFilter, TSelection> where TFilter : new()
    {
        private List<TData> _filteredData { get; set; } = new();
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        [Parameter]
        public bool IsLoading { get; set; } = false;
        [Parameter]
        public EventCallback<bool> IsLoadingChanged { get; set; }
        [Parameter]
        public List<DropdownItemModel> Columns { get; set; } = new();
        [Parameter]
        public List<ErrorDetailDto> Errors { get; set; } = new List<ErrorDetailDto>();
        [Parameter]
        public EventCallback<List<ErrorDetailDto>> ErrorsChanged { get; set; }

        [Parameter]
        public RenderFragment? FilterContent { get; set; }

        [Parameter]
        public RenderFragment? ActionContent { get; set; }

        [Parameter]
        public Dictionary<string, RenderFragment<TData>> CellContents { get; set; } = new Dictionary<string, RenderFragment<TData>>();

        private int _totalPages { get; set; } = 0;
        private int _totalItems { get; set; } = 0;

        [Parameter]
        public string ApiEndpoint { get; set; } = string.Empty;

        [Parameter]
        public CHttpClientType HttpClientType { get; set; }

        [Parameter]
        public CPortalType PortalType { get; set; }
        [Parameter]
        public TFilter FilterProperty { get; set; } = new TFilter();
        [Parameter]
        public EventCallback<TFilter> FilterPropertyChanged { get; set; }

        [Parameter]
        public TableParam<TFilter> RequestData { get; set; } = new TableParam<TFilter>();
        [Parameter]
        public EventCallback<TableParam<TFilter>> RequestDataChanged { get; set; }

        private bool _showFilterPanel { get; set; } = false;


        [Parameter]
        public bool IsEnableSelection { get; set; } = false;

        [Parameter]
        public List<TSelection> SelectedItems { get; set; } = new List<TSelection>();

        [Parameter]
        public EventCallback<List<TSelection>> SelectedItemsChanged { get; set; }

        [Parameter]
        public Func<TData, TSelection>? MapSelection { get; set; }

        [Parameter]
        public string SearchPlaceHolder { get; set; } = "Search....";

        [Parameter]
        public string FilterLabelName { get; set; } = "Filter";

        protected override async Task OnInitializedAsync()
        {
            await FetchDataAsync();
        }

        private async Task FetchDataAsync()
        {
            try
            {
                IsLoading = true;
                await IsLoadingChanged.InvokeAsync(IsLoading);

                RequestData.PageNumber = _currentPage;
                RequestData.PageSize = _itemsPerPage;

                await RequestDataChanged.InvokeAsync(RequestData);

                var apiResponse = await _httpClientHelper.PostAsync<TableParam<TFilter>, BasePagedResult<TData>>(
                    endpoint: ApiEndpoint,
                    data: RequestData,
                    requestType: HttpClientType,
                    portalType: PortalType);

                if (apiResponse == null)
                {
                    _toastService.ShowError($"Không thể kết nối đến server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!apiResponse.Errors.IsNullOrEmpty())
                    {
                        Errors = apiResponse.Errors;
                        await ErrorsChanged.InvokeAsync(Errors);
                    }
                    else if (apiResponse.Data == null)
                    {
                        _toastService.ShowError($"Đã có lỗi xảy ra trong quá trình lấy dữ liệu.");
                    }
                    else
                    {
                        _filteredData.Clear();
                        _filteredData = new List<TData>(apiResponse.Data.Items);
                        Columns = Columns.IsNullOrEmpty() ? GetColumnsFromData(_filteredData) : Columns;
                        if (!Columns.Any(s => s.Name == "#"))
                        {
                            if (IsEnableSelection && MapSelection != null)
                            {
                                Columns.Insert(index: 0, item: new DropdownItemModel()
                                {
                                    Name = "#",
                                    Value = "#",
                                    IsSelected = true,
                                    IsSort = false
                                });
                            }
                        }
                        _totalItems = apiResponse.Data.TotalItems;
                        _totalPages = apiResponse.Data.TotalPages;
                    }
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowError(ex.Message);
            }
            finally
            {
                IsLoading = false;
                await IsLoadingChanged.InvokeAsync(IsLoading);
            }
        }

        private List<DropdownItemModel> GetColumnsFromData(List<TData> data)
        {
            if (!data.IsNullOrEmpty())
            {
                TData firstItem = data.First();
                return firstItem!.GetType().GetProperties().Select(p => new DropdownItemModel
                {
                    Name = p.Name,
                    Value = p.Name,
                    IsSelected = true,
                    IsSort = true
                }).ToList();
            }
            return new();
        }

        #region Search action
        private async Task Search(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await FetchDataAsync();
                StateHasChanged();
            }
        }

        private void OnSearchInputChange(ChangeEventArgs e)
        {
            RequestData.SearchQuery = e.Value?.ToString() ?? string.Empty;
            StateHasChanged();
        }

        #endregion Search action

        #region paging action
        private bool CanNavigatePrevious => _currentPage > 1;

        private bool CanNavigateNext => _currentPage < _totalPages;

        private bool CanNavigateFirst => _currentPage > 1;

        private bool CanNavigateLast => _currentPage < _totalPages;

        private List<int> GetPagingNumbers()
        {
            var start = Math.Max(1, _currentPage - 2);
            var end = Math.Min(_totalPages, _currentPage + 2);
            var pages = new List<int>();

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }

            return pages;
        }

        private void ApplyPaging()
        {
            _filteredData = _filteredData.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
        }

        private async Task NextPage()
        {
            if (CanNavigateNext)
            {
                _currentPage++;
                RequestData.PageNumber = _currentPage;
                await FetchDataAsync();
            }
        }

        private async Task PreviousPage()
        {
            if (CanNavigatePrevious)
            {
                _currentPage--;
                RequestData.PageNumber = _currentPage;
                await FetchDataAsync();
            }
        }

        private async Task FirstPage()
        {
            if (CanNavigateFirst)
            {
                _currentPage = 1;
                RequestData.PageNumber = _currentPage;
                await FetchDataAsync();
            }
        }

        private async Task LastPage()
        {
            if (CanNavigateLast)
            {
                _currentPage = _totalPages;
                RequestData.PageNumber = _currentPage;
                await FetchDataAsync();
            }
        }

        private async Task GoToPage(int page)
        {
            if (page > 0 && page <= _totalPages)
            {
                _currentPage = page;
                RequestData.PageNumber = _currentPage;
                await FetchDataAsync();
            }
        }

        private async Task HandlePageChange(int itemsPerPage)
        {
            _itemsPerPage = itemsPerPage;
            await FetchDataAsync();
        }
        #endregion paging action

        #region Filter action
        private async Task HandleFilterApplied()
        {
            RequestData.Filter = FilterProperty;
            _currentPage = 1;
            await FetchDataAsync();
        }

        private void ToggleFilterPanel()
        {
            _showFilterPanel = !_showFilterPanel;
        }
        #endregion Filter action

        private void ApplyColumnSelection(List<DropdownItemModel> selectedColumns)
        {
            foreach (var column in Columns)
            {
                column.IsSelected = selectedColumns.Any(c => c.Name == column.Name && c.IsSelected);
            }
        }

        private void CancelColumnSelection()
        {
            _showFilterPanel = false;
        }

        private async Task SortAsync(string propertyName)
        {
            if (RequestData.Sorter == null)
            {
                RequestData.Sorter = new();
            }
            RequestData.Sorter.KeyName = propertyName;
            RequestData.Sorter.IsASC = !RequestData.Sorter.IsASC;
            await FetchDataAsync();
        }



        private async Task OnToggleSelectedItem(TData row)
        {
            if (MapSelection is null) return;

            var selectionItem = MapSelection.Invoke(row);

            if (SelectedItems.Contains(selectionItem))
            {
                SelectedItems.Remove(selectionItem);
            }
            else
            {
                SelectedItems.Add(selectionItem);
            }
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
        }
    }
}