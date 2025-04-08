using MMA.Domain;

namespace MMA.Service
{
    public interface IMegaService
    {
        Task<BasePagedResult<MegaAccountDetailDto>> GetMegaAccountWithPagingAsync(TableParam<MegaAccountFilterProperty> tableParam);
        Task MegaLoginAsync(LoginRequestDto requestDto);
    }
}