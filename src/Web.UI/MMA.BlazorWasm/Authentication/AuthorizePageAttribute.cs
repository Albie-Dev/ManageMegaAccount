using Microsoft.AspNetCore.Authorization;
using MMA.Domain;

namespace MMA.BlazorWasm
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizePageAttribute : AuthorizeAttribute
    {
        public CRoleType? Role { get; }
        public CResourceType? Resource { get; }
        public CPermissionType? Permission { get; }

        public AuthorizePageAttribute(CRoleType role)
        {
            Role = role;
        }

        public AuthorizePageAttribute(CRoleType role, CResourceType resource)
        {
            Role = role;
            Resource = resource;
        }

        public AuthorizePageAttribute(CRoleType role, CResourceType resource, CPermissionType permission)
        {
            Role = role;
            Resource = resource;
            Permission = permission;
        }
    }
}
