using Microsoft.AspNetCore.Components.Forms;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages
{
    public partial class Home
    {
        private List<UserRoleProperty> Roles { get; set; } = new List<UserRoleProperty>();
        private NotificationResponse? _notificationResponse { get; set; }
        private UserRoleProperty? selectedRole;
        HashSet<ResourceProperty> expandedResources = new();
        private int selectedTabIndex = 0;
        private bool _isLoading { get; set; } = false;

        public bool IsOpen { get; set; } = false;

        public List<CFileType> FileTypes = new List<CFileType> {
            CFileType.Excel,
            CFileType.Audio,
            CFileType.Database,
            CFileType.Image,
            CFileType.TXT,
            CFileType.Other,
            CFileType.Video,
            CFileType.PowerPoint,
            CFileType.Word,
            CFileType.Zip,
            CFileType.PDF,
            CFileType.Html,
            CFileType.MicrosoftDownlod
        };

        private void ShowModal()
        {
            IsOpen = true;
        }

        private async Task HandleUploadFromModal(IBrowserFile file)
        {
            var response = await _httpClientHelper.UploadFileAsync(file, "api/FileUpload/upload");
            if (response.IsSuccessStatusCode)
            {

            }
            else
            {

            }
        }

        private async Task UpdateUserRoleAsync()
        {
            try
            {
                var apiResponse = await _httpClientHelper.PostAsync<AddUserRoleRequestDto, NotificationResponse>(
                  endpoint: Path.Combine(EndpointConstant.CET_Base_Url, EndpointConstant.CET_UserRole_Update),
                  data: new AddUserRoleRequestDto()
                  {
                      UserId = new Guid("7604fd9c-2739-4e59-b765-6b225bb7ebf5"),
                      UserRoles = Roles
                  },
                  requestType: CHttpClientType.Private,
                  portalType: CPortalType.CET);

                if (apiResponse != null && apiResponse.Data != null)
                {
                    _notificationResponse = apiResponse.Data;
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
        private void OnPermissionChange(UserRoleProperty roleSelected, ResourceProperty resource, PermissionProperty permission, object? newValue)
        {
            if (bool.TryParse(newValue?.ToString(), out bool isChecked))
            {
                permission.HasPermission = isChecked;
                int permissionCount = 0;
                foreach (var role in Roles)
                {
                    if (role.RoleId == roleSelected.RoleId)
                    {
                        foreach (var rs in role.Resources)
                        {
                            if (rs.ResourceType == resource.ResourceType)
                            {
                                foreach (var pt in rs.PermissionTypes)
                                {
                                    if (pt == permission)
                                    {
                                        pt.HasPermission = isChecked;
                                    }
                                    if (isChecked || pt.HasPermission)
                                    {
                                        permissionCount++;
                                    }
                                }
                            }
                        }
                        role.HasRole = permissionCount > 0;
                    }
                };
            }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var apiResponse = await _httpClientHelper.GetAsync<List<UserRoleProperty>>(
                endpoint: "https://localhost:7201/api/v1/cet/userrole/7604fd9c-2739-4e59-b765-6b225bb7ebf5",
                requestType: CHttpClientType.Private,
                portalType: CPortalType.CET);

                if (apiResponse == null)
                {
                    _toastService.ShowError($"Cannot connect to server. Host = {CPortalType.CET.ToDescription()}");
                }
                else 
                {
                    if (apiResponse.Data == null)
                    {
                        _toastService.ShowError($"Đã có lỗi xả ra trong quá trình lấy dữ liệu. Errors = {apiResponse.Errors.Select(s => s.Error).ToList().ToMultilineString()}");
                    }
                    else
                    {
                        Roles = apiResponse.Data;
                        if (Roles.Count > 0)
                        {
                            selectedRole = Roles[0];
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
                _isLoading = false;
            }
        }

        private void SelectTab(int index)
        {
            if (Roles != null && index >= 0 && index < Roles.Count)
            {
                selectedTabIndex = index;
                selectedRole = Roles[index];
            }
        }

        void ToggleExpandResource(ResourceProperty resource)
        {
            if (!expandedResources.Add(resource))
                expandedResources.Remove(resource);
        }
    }
}