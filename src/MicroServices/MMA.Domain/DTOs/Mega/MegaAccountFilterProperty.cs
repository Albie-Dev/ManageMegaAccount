namespace MMA.Domain
{
    public class MegaAccountFilterProperty
    {
        public DateTimeOffset? FromCreatedDate { get; set; }
        public DateTimeOffset? ToCreatedDate { get; set; }
        public DateTimeOffset? ToLastLogin { get; set; }
        public DateTimeOffset? FromLastLogin { get; set; }
        public DateTimeOffset? ToExpiredDate { get; set; }
        public DateTimeOffset? FromExpiredDate { get; set; }
        public DateTimeOffset? ToModifiedDate { get; set; }
        public DateTimeOffset? FromModifiedDate { get; set; }
    }
}