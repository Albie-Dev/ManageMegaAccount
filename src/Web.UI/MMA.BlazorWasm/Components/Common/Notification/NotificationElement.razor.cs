using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Notification
{
    public partial class NotificationElement
    {
        [Parameter]
        public int NewNotificationCount { get; set; } = 0;
        [Parameter]
        public EventCallback<int> NewNotificationCountChanged { get; set; }
        [Parameter]
        public string IconDefault { get; set; } = "bi bi-bell";
        [Parameter]
        public string IconChanged { get; set; } = "bi bi-bell-fill";

        private List<NotificationItemModel> _items { get; set; } = new List<NotificationItemModel>();
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private TableParam<BaseFilter> _tableParam { get; set; } = new TableParam<BaseFilter>();
        private bool _isLoading { get; set; } = false;

        private bool _isToggle { get; set; } = false;
        private string _notificationDropdownId { get; set; } = $"noftiDropdownId{Guid.NewGuid()}";
        private async Task ToggleViewNotificationAsync()
        {
            _isToggle = !_isToggle;
            if (_isToggle)
            {
                NewNotificationCount = 0;
                await NewNotificationCountChanged.InvokeAsync(NewNotificationCount);
                try
                {
                    var apiResponse = await _httpClientHelper.PostAsync<TableParam<BaseFilter>, BasePagedResult<NotificationItemModel>>(
                        endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Notification_PagingItem),
                        data: _tableParam,
                        requestType: CHttpClientType.Private,
                        portalType: CPortalType.CET);
                    if (apiResponse == null)
                    {
                        _toastService.ShowError(message: $"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                    }
                    else
                    {
                        if (!apiResponse.Errors.IsNullOrEmpty())
                        {
                            _errors = apiResponse.Errors;
                        }
                        else if (apiResponse.Data == null)
                        {
                            _toastService.ShowError(message: $"An error occured while fetch data. Server no response data.");
                        }
                        else
                        {
                            _items = apiResponse.Data.Items;
                        }
                    }
                }
                catch(Exception ex)
                {
                    _toastService.ShowError(message: $"{ex.Message}. Host = {CPortalType.CET.ToDescription()}");
                }
                finally
                {
                    _isLoading = false;
                }
            }
        }
    }
}