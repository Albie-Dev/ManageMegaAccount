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

            var requestRoleIds = requestDto.UserRoles.Select(s => s.RoleId).ToList();
            var userRoles = await _repository.GetAsync<UserRoleEntity>(s => s.UserId == requestDto.UserId);

            var deletedUserRoles = userRoles.Where(s => !requestRoleIds.Contains(s.RoleId)).ToList();
            if (!deletedUserRoles.IsNullOrEmpty())
            {
                await _repository.DeleteRangeAsync<UserRoleEntity>(deletedUserRoles);
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
                await _repository.AddRangeAsync<UserRoleEntity>(newUserRoles);
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

                await _repository.UpdateRangeAsync<UserRoleEntity>(updateUserRoles);
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
            var allRoles = await _roleService.GetAllRolesAsync();
            var roles = Enum.GetValues(typeof(CRoleType)).Cast<CRoleType>();
            var resourceTypes = Enum.GetValues(typeof(CResourceType)).Cast<CResourceType>();
            var permissionTypes = Enum.GetValues(typeof(CPermissionType)).Cast<CPermissionType>();

            var result = new List<UserRoleProperty>();

            foreach (var role in roles)
            {
                if (role != CRoleType.None)
                {
                    var roleInfo = allRoles.FirstOrDefault(s => s.RoleType == role);
                    var dto = new UserRoleProperty
                    {
                        RoleId = roleInfo?.RoleId ?? Guid.Empty,
                        RoleName = roleInfo?.RoleName ?? string.Empty,
                        RoleType = role,
                        HasRole = false,
                        Resources = resourceTypes.Where(s => s != CResourceType.None).Select(resource => new ResourceProperty
                        {
                            ResourceName = resource.ToString(),
                            ResourceType = resource,
                            PermissionTypes = permissionTypes.Where(s => s != CPermissionType.None).Select(permission => new PermissionProperty
                            {
                                PermissionName = permission.ToString(),
                                PermissionType = permission,
                                HasPermission = false
                            }).ToList()
                        }).ToList()
                    };

                    result.Add(dto);
                }
            }
            return result;
        }
    }
}