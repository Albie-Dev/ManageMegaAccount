using MMA.Domain;

namespace MMA.Service
{
    public interface IUserService
    {
        Task<BasePagedResult<UserBaseInfoDto>> GetUsersForFilterAsync(TableParam<BaseFilter> tableParam);
    }
}