using MMA.Domain;

namespace MMA.Service
{
    public interface IActorService
    {
        Task<BasePagedResult<UserBaseInfoDto>> GetActorBaseInfoASync(TableParam<BaseFilter> tableParam);
        Task<BasePagedResult<ActorDetailDto>> GetActorWithPagingAsync(TableParam<ActorFilterProperty> tableParam);
        Task<ActorDetailDto> GetActorDetaiAsync(Guid actorId);
        Task<NotificationResponse> AddActorAsync(CreateActorRequestDto actorRequestDto);
        Task<NotificationResponse> UpdateActorAsync(UpdateActorRequestDto actorRequestDto);
        Task<NotificationResponse> DeleteActorAsync(DeleteActorRequestDto actorRequestDto);
        Task<NotificationResponse> DeactiveActorAsync(DeactiveActorRequestDto actorRequestDto);
        Task<byte[]> ExportActorByExcelTemplateAsyn(TableParam<ActorFilterProperty> tableParam);
    }
}