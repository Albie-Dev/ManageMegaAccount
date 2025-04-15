namespace MMA.Domain
{
    public class DeactiveActorRequestDto
    {
        public List<Guid> ActorIds { get; set; } = new List<Guid>();
    }
}