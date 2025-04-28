using MMA.Domain;

namespace MMA.Service
{
    public interface IUserService
    {
        Task<UserBaseInfoDto> GetUserBaseInfoAsync();
        Task<BasePagedResult<UserBaseInfoDto>> GetUsersForFilterAsync(TableParam<BaseFilter> tableParam);
    }
}