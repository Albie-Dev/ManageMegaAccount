using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Role.Paging
{
    public partial class Index
    {
        private List<ErrorDetailDto> _errors { get; set; } = new List<ErrorDetailDto>();
        private RoleFilterProperty _roleFilterProperty { get; set; } = new RoleFilterProperty();
        private List<Guid> _selectedRoleIds { get; set; } = new List<Guid>();
        private bool _isLoading { get; set; } = false;
        private NotificationResponse? _notificationResponse { get; set; }

        #region Sync new Role
        private async Task SyncNewRolesAsync()
        {
            try
            {
                _isLoading = true;
                var apiResponse = await _httpClientHelper.PostAsync<NotificationResponse>(
                    endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_Role_Sync),
                    requestType: CHttpClientType.Private,
                    portalType: CPortalType.CET);
                if (apiResponse == null)
                {
                    _toastService.ShowError($"Không thể kết nối đến server. Host = {CPortalType.CET.ToDescription()}");
                }
                else
                {
                    if (!apiResponse.Errors.IsNullOrEmpty())
                    {
                        _errors = apiResponse.Errors;
                    }
                    else if (apiResponse.Data == null)
                    {
                        _toastService.ShowError($"Đã có lỗi xảy ra trong quá trình sync vai trò mới.");
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
        #endregion Sync new Role
    }
}