namespace MMA.Domain
{
    public class UserRoleProperty
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public CRoleType RoleType { get; set; }
        public bool HasRole { get; set; }
        public List<ResourceProperty> Resources { get; set; } = new List<ResourceProperty>();
    }

    public class ResourceProperty
    {
        public string ResourceName { get; set; } = string.Empty;
        public CResourceType ResourceType { get; set; }
        public List<PermissionProperty> PermissionTypes { get; set; } = new List<PermissionProperty>();
    }

    public class PermissionProperty
    {
        public bool HasPermission { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public CPermissionType PermissionType { get; set; }
    }
}