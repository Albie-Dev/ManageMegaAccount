
namespace MMA.Domain
{
    public class MegaAccountDetailDto
    {
        public Guid MegaAccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string RecoveryKey { get; set; } = string.Empty;
        public DateTimeOffset LastLogin { get; set; }
        public DateTimeOffset ExpiredDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public UserBaseInfoDto CreatedUserInfo { get; set; } = null!;
        public UserBaseInfoDto ModifiedUserInfo { get; set; } = null!;
    }
}