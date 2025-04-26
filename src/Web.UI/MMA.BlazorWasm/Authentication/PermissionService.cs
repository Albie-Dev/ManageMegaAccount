using System.Security.Claims;
using MMA.Domain;

namespace MMA.BlazorWasm
{

    public class PermissionService
    {
        private ClaimsPrincipal? _user;

        public void SetUser(ClaimsPrincipal user)
        {
            _user = user;
        }

        public bool HasPermission(CRoleType? role, CResourceType? resource = null, CPermissionType? permission = null)
        {
            if (_user == null || _user.Identity == null || !_user.Identity.IsAuthenticated)
                return false;

            var roleClaims = _user.FindAll(ClaimTypes.Role)
                .Select(rc => rc.Value.FromJson<RoleClaimDto>()).ToList();

            var roleClaim = roleClaims.FirstOrDefault(rc => rc.RoleName == role.ToString());
            if (roleClaim == null)
                return false;

            if (!resource.HasValue && !permission.HasValue)
            {
                return true;
            }

            if (resource.HasValue)
            {
                var resourcePermission = roleClaim.RolePermissions
                    .FirstOrDefault(rp => rp.ResourceType == resource);

                if (resourcePermission == null)
                    return false;

                if (permission.HasValue)
                {
                    return resourcePermission.PermissionTypes.Contains(permission.Value);
                }

                return true;
            }

            return false;
        }
    }
}