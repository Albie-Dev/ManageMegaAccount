using MMA.Domain;

namespace MMA.Service
{
    public interface IMegaService
    {
        Task<BasePagedResult<MegaAccountDetailDto>> GetMegaAccountWithPagingAsync(TableParam<MegaAccountFilterProperty> tableParam);
        Task<(Dictionary<string, ImportResult<object>>, byte[])> ImportMegaAccountsAsync(Stream fileStream);
        Task MegaLoginAsync(LoginRequestDto requestDto);
    }
}