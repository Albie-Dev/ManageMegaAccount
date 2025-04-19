using MMA.Domain;
using MMA.Service;

namespace MMA.Service
{
    public interface IRoleService
    {
        Task<BasePagedResult<RoleDetailResponseDto>> GetWithPagingAsync(TableParam<RoleFilterProperty> tableParam);
        Task<NotificationResponse> SyncRolesAsync();
    }
}