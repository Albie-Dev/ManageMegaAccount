using MMA.Domain;

namespace MMA.Service
{
    public interface IMegaAccountService
    {
        Task<BasePagedResult<MegaAccountDetailDto>> GetMegaAccountWithPagingAsync(TableParam<MegaAccountFilterProperty> tableParam);
        Task<byte[]> DownloadMegaAccountImportTemplateAsync();
        Task<byte[]> ImportMegaAccountsAsync(Stream fileStream);
        Task MegaLoginAsync(LoginRequestDto requestDto);
        Task<NotificationResponse> SyncMegaAccountDataAsync(Guid megaAccountId);
        Task<NotificationResponse> LoginMegaAccountWithIdAsync(MegaAccountLoginRequestDto requestDto);
        Task<List<FileProperty>> GetMegaAccountDataAsync(Guid megaAccountId);
    }
}