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
            Policy = BuildPolicy();
        }

        public AuthorizePageAttribute(CRoleType role, CResourceType resource)
        {
            Role = role;
            Resource = resource;
            Policy = BuildPolicy();
        }

        public AuthorizePageAttribute(CRoleType role, CResourceType resource, CPermissionType permission)
        {
            Role = role;
            Resource = resource;
            Permission = permission;
            Policy = BuildPolicy();
        }

        private string BuildPolicy()
        {
            var parts = new List<string>();

            if (Role.HasValue)
                parts.Add(Role.Value.ToString());

            if (Resource.HasValue)
                parts.Add(Resource.Value.ToString());

            if (Permission.HasValue)
                parts.Add(Permission.Value.ToString());

            // The resulting policy name will be something like "Admin.Movie.Manage"
            return string.Join(".", parts);
        }
    }
}
