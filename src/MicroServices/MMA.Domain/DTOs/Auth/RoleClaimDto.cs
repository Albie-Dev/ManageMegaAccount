namespace MMA.Domain
{
    public class RoleClaimDto
    {
        public string RoleName { get; set; } = string.Empty;
        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}