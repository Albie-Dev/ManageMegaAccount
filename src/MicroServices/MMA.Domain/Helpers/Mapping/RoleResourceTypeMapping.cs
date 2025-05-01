namespace MMA.Domain
{
    public class RoleResourceTypeMapping
    {
        public static readonly Dictionary<CRoleType, Dictionary<CResourceType, List<CPermissionType>>> DefaultRoleResources = new()
        {
            {
                CRoleType.Admin, new Dictionary<CResourceType, List<CPermissionType>>
                {
                    {
                        CResourceType.User, new List<CPermissionType>
                        {
                            CPermissionType.Read,
                            CPermissionType.Update,
                            CPermissionType.Delete,
                            CPermissionType.Manage
                        }
                    },
                    {
                        CResourceType.Role,
                        new List<CPermissionType>
                        {
                            CPermissionType.Read,
                            CPermissionType.Update,
                            CPermissionType.Delete,
                            CPermissionType.Manage
                        }
                    },
                    {
                        CResourceType.Actor,
                        new List<CPermissionType>
                        {
                            CPermissionType.Read,
                            CPermissionType.Update,
                            CPermissionType.Delete,
                            CPermissionType.Manage
                        }
                    }
                }
            },
            {
                CRoleType.Client,
                new Dictionary<CResourceType, List<CPermissionType>>
                {
                    {
                        CResourceType.User,
                        new List<CPermissionType>
                        {
                            CPermissionType.Read,
                        }
                    },
                    {
                        CResourceType.Role,
                        new List<CPermissionType>
                        {
                            CPermissionType.Read,
                        }
                    },
                    {
                        CResourceType.Actor,
                        new List<CPermissionType>
                        {
                            CPermissionType.Read,
                        }
                    }
                }
            }
        };

    }
}