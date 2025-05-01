using MMA.Domain;

namespace MMA.Service
{
    public interface IRoleService
    {
        Task<BasePagedResult<RoleDetailResponseDto>> GetWithPagingAsync(TableParam<RoleFilterProperty> tableParam);
        Task<NotificationResponse> SyncRolesAsync();
        Task<List<RoleDetailResponseDto>> GetAllRolesAsync();
    }
}