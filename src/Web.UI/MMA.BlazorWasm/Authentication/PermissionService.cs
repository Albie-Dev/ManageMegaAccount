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

        public bool HasPermission(CRoleType role, CResourceType resource, CPermissionType permission)
        {
            if (_user == null || _user.Identity == null || !_user.Identity.IsAuthenticated)
                return false;

            var roleClaims = _user.FindAll(ClaimTypes.Role)
                .Select(rc => rc.Value.FromJson<RoleClaimDto>()).ToList();

            return roleClaims.Any(rc =>
                rc.RoleName == role.ToString() &&
                rc.RolePermissions.Any(rp =>
                    rp.ResourceType == resource &&
                    rp.PermissionTypes.Contains(permission)));
        }
    }
}