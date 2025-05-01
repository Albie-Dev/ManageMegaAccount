namespace MMA.Domain
{
    public class UserClaimDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<RoleClaimDto> RoleClaims { get; set; } = new List<RoleClaimDto>();
        public bool IsAuthenticated { get; set; }
    }
}