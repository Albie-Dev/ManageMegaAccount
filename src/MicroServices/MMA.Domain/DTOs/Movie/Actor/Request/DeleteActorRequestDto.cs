namespace MMA.Domain
{
    public class DeleteActorRequestDto
    {
        public Guid ActorId { get; set; }
        public List<Guid> SubActorIds { get; set; } = new();
    }
}