using MMA.Domain;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MMA.Service
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class MMAAuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        public CRoleType Role { get; set; }
        public CResourceType Resource { get; set; }
        public CPermissionType Permission { get; set; }

        public MMAAuthorizedAttribute(CRoleType role, CResourceType resource, CPermissionType permission)
        {
            Role = role;
            Resource = resource;
            Permission = permission;
        }

        public MMAAuthorizedAttribute(){}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var rolesClaims = user.FindAll(ClaimTypes.Role).Select(s => s.Value.FromJson<RoleClaimDto>()).ToList();
            if (rolesClaims.IsNullOrEmpty())
            {
                context.Result = new ForbidResult();
                return;
            }

            bool hasPermission = false;

            foreach (var roleClaim in rolesClaims)
            {
                try
                {
                    if (Role == CRoleType.None)
                    {
                        hasPermission = true;
                        break;
                    }
                    Enum.TryParse(roleClaim.RoleName, out CRoleType roleType);
                    if (roleType == Role)
                    {
                        foreach (var rolePermission in roleClaim.RolePermissions)
                        {
                            if (rolePermission.ResourceType == Resource && rolePermission.PermissionTypes.Contains(Permission))
                            {
                                hasPermission = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                if (hasPermission) break;
            }

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}