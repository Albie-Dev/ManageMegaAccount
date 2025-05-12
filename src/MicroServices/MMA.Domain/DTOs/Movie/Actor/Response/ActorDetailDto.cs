namespace MMA.Domain
{
    public class ActorDetailDto : BaseActorInfoDto
    {
        public Guid ActorId { get; set; }
        public int TotalView { get; set; }
        public int TotalMovie { get; set; }
        public double RateStar { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public UserBaseInfoDto? CreatedByProperty { get; set; }
        public UserBaseInfoDto? ModifiedByProperty { get; set; }
    }
}