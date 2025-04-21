using MMA.Domain;

namespace MMA.Service
{
    public interface IUserRoleService
    {
        Task<List<UserRoleProperty>> InitRoleResourcePermission();
        Task<List<UserRoleProperty>> GetUserRoleResourcePermissionAsync(Guid userId);
        Task<NotificationResponse> AddUserRoleResourcePermissionsAsync(AddUserRoleRequestDto requestDto);
    }
}