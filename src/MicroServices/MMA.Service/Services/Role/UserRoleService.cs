using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MMA.Domain;
using static MMA.Domain.ModelStateValidExtension;

namespace MMA.Service
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IDbRepository _repository;
        private readonly ILogger<UserRoleService> _logger;
        private readonly IRoleService _roleService;

        public UserRoleService(
            IDbRepository repository,
            ILogger<UserRoleService> logger,
            IRoleService roleService)
        {
            _repository = repository;
            _logger = logger;
            _roleService = roleService;
        }


        public async Task<List<UserRoleProperty>> GetUserRoleResourcePermissionAsync(Guid userId)
        {
            var modelState = new CustomModelState();
            var userRolePermissions = await _repository.GetAsync<UserRoleEntity>(
                predicate: s => s.UserId == userId
            );

            var allUserRoleResourcePermissions = await InitRoleResourcePermission();

            if (userRolePermissions.IsNullOrEmpty())
            {
                _logger.LogInformation($"User with ID = {userId} don't have any roles or permission");
                return allUserRoleResourcePermissions;
            }

            foreach (var roleDto in allUserRoleResourcePermissions)
            {
                var userRole = userRolePermissions.FirstOrDefault(s => s.RoleId == roleDto.RoleId);
                if (userRole != null)
                {
                    roleDto.HasRole = true;

                    foreach (var resourceDto in roleDto.Resources)
                    {
                        var matchingPermissions = userRole.RolePermissions
                            .Where(p => p.ResourceType == resourceDto.ResourceType)
                            .SelectMany(p => p.PermissionTypes)
                            .ToHashSet();

                        foreach (var permission in resourceDto.PermissionTypes)
                        {
                            if (matchingPermissions.Contains(permission.PermissionType))
                            {
                                permission.HasPermission = true;
                            }
                        }
                    }
                }
            }

            return allUserRoleResourcePermissions;
        }


        public async Task<NotificationResponse> AddUserRoleResourcePermissionsAsync(AddUserRoleRequestDto requestDto)
        {
            var modelState = requestDto.ModelStateValidate();
            if (!modelState.GetErrors().IsNullOrEmpty())
            {
                throw new MMAException(statusCode: StatusCodes.Status400BadRequest, errors: modelState.GetErrors());
            }

            requestDto.UserRoles.RemoveAll(s => !s.HasRole);
            requestDto.UserRoles.ForEach(role => role.Resources.ForEach(resource => resource.PermissionTypes.RemoveAll(s => !s.HasPermission)));

            var requestRoleIds = requestDto.UserRoles.Select(s => s.RoleId).ToList();
            var userRoles = await _repository.GetAsync<UserRoleEntity>(s => s.UserId == requestDto.UserId);

            var deletedUserRoles = userRoles.Where(s => !requestRoleIds.Contains(s.RoleId)).ToList();
            if (!deletedUserRoles.IsNullOrEmpty())
            {
                await _repository.DeleteRangeAsync<UserRoleEntity>(deletedUserRoles, clearTracker: true);
                _logger.LogInformation($@"{nameof(UserRoleService)} {nameof(AddUserRoleResourcePermissionsAsync)}:
            deleted. Count = {deletedUserRoles.Count}, RoleIds = {string.Join(",", deletedUserRoles.Select(s => s.RoleId))}");
            }

            var existingRoleIds = userRoles.Select(s => s.RoleId).ToHashSet();
            var newUserRoles = requestDto.UserRoles
                .Where(s => !existingRoleIds.Contains(s.RoleId))
                .Select(userRole => new UserRoleEntity
                {
                    UserId = requestDto.UserId,
                    RoleId = userRole.RoleId,
                    Status = CMasterStatus.Active,
                    RolePermissionProperty = SerializeRolePermissions(userRole.Resources)
                }).ToList();

            if (!newUserRoles.IsNullOrEmpty())
            {
                await _repository.AddRangeAsync<UserRoleEntity>(newUserRoles, clearTracker: true);
                _logger.LogInformation($@"{nameof(UserRoleService)} {nameof(AddUserRoleResourcePermissionsAsync)}:
            added. Count = {newUserRoles.Count}, RoleIds = {string.Join(",", newUserRoles.Select(s => s.RoleId))}");
            }

            var updateUserRoles = userRoles.Where(s => requestRoleIds.Contains(s.RoleId)).ToList();
            if (!updateUserRoles.IsNullOrEmpty())
            {
                foreach (var userRole in updateUserRoles)
                {
                    var updateDto = requestDto.UserRoles.FirstOrDefault(s => s.RoleId == userRole.RoleId);
                    if (updateDto != null)
                    {
                        userRole.RolePermissionProperty = SerializeRolePermissions(updateDto.Resources);
                    }
                }

                await _repository.UpdateRangeAsync<UserRoleEntity>(updateUserRoles, clearTracker: true);
                _logger.LogInformation($@"{nameof(UserRoleService)} {nameof(AddUserRoleResourcePermissionsAsync)}:
            updated. Count = {updateUserRoles.Count}, RoleIds = {string.Join(",", updateUserRoles.Select(s => s.RoleId))}");
            }

            return new NotificationResponse
            {
                Level = CNotificationLevel.Success,
                DisplayType = CNotificationDisplayType.Page,
                Message = $"",
                Type = CNotificationType.Update
            };
        }

        private string SerializeRolePermissions(List<ResourceProperty> roleResources)
        {
            return roleResources.Select(s => new RolePermission
            {
                ResourceType = s.ResourceType,
                PermissionTypes = s.PermissionTypes.Select(s => s.PermissionType).ToList()
            }).ToJson();
        }



        public async Task<List<UserRoleProperty>> InitRoleResourcePermission()
        {
            // Lấy tất cả các role từ database (hoặc API)
            var allRoles = await _roleService.GetAllRolesAsync();

            // Danh sách kết quả
            var result = new List<UserRoleProperty>();

            // Duyệt qua tất cả các role trong _map
            foreach (var roleEntry in RoleResourceTypeMapping.DefaultRoleResources)
            {
                var role = roleEntry.Key; // CRoleType (Admin, Client...)

                // Bỏ qua role "None" (không có quyền)
                if (role != CRoleType.None)
                {
                    // Lấy thông tin role từ database (hoặc nguồn dữ liệu)
                    var roleInfo = allRoles.FirstOrDefault(s => s.RoleType == role);

                    // Khởi tạo đối tượng UserRoleProperty cho role hiện tại
                    var dto = new UserRoleProperty
                    {
                        RoleId = roleInfo?.RoleId ?? Guid.Empty,
                        RoleName = roleInfo?.RoleName ?? string.Empty,
                        RoleType = role,
                        HasRole = false, // Mặc định là không có quyền
                        Resources = new List<ResourceProperty>()
                    };

                    // Lấy dữ liệu phân quyền từ _map cho Role hiện tại
                    var resourcePermissions = roleEntry.Value; // Dictionary<CResourceType, List<CPermissionType>>

                    // Duyệt qua các resource của role
                    foreach (var resourceEntry in resourcePermissions)
                    {
                        var resourceType = resourceEntry.Key; // ResourceType (User, Role, Actor...)
                        var permissions = resourceEntry.Value; // List<CPermissionType> (Manage, Read, Update...)

                        // Tạo một ResourceProperty tương ứng với ResourceType
                        var resourceProperty = new ResourceProperty
                        {
                            ResourceName = resourceType.ToString(),
                            ResourceType = resourceType,
                            PermissionTypes = new List<PermissionProperty>()
                        };

                        // Duyệt qua các permission của resource và gán vào resourceProperty
                        foreach (var permission in permissions)
                        {
                            var permissionProperty = new PermissionProperty
                            {
                                PermissionName = permission.ToString(),
                                PermissionType = permission,
                                HasPermission = false // Mặc định không có quyền
                            };
                            resourceProperty.PermissionTypes.Add(permissionProperty);
                        }

                        // Thêm resourceProperty vào Resources của dto
                        dto.Resources.Add(resourceProperty);
                    }

                    // Thêm dto vào kết quả
                    result.Add(dto);
                }
            }

            return result;
        }
    }
}