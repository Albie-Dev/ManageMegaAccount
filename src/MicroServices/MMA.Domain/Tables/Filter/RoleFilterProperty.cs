namespace MMA.Domain
{
    public class RoleFilterProperty
    {
        public List<CRoleType> RoleTypes { get; set; } = new();
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}