namespace MMA.Domain
{
    public class UpdateActorRequestDto : BaseActorInfoDto
    {
        public Guid ActorId { get; set; }
        public List<Guid> SubActorIds { get; set; } = new List<Guid>();
    }
}