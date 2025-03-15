namespace MMA.Domain
{
    public class GetActorDetailRequestDto
    {
        public Guid ActorId { get; set; }
        public Guid? SubActorId { get; set; }
    }
}