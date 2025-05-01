using System.Security.Claims;

namespace MMA.Domain
{
    public static class ClaimPrincipalExtension
    {
        public static UserClaimDto ToUserClaim(this ClaimsPrincipal user)
        {
            if (user == null || (!user.Identity?.IsAuthenticated ?? true))
            {
                return new UserClaimDto { IsAuthenticated = false };
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;

            var roleClaimValues = user.FindAll(ClaimTypes.Role).Select(rcv => rcv.Value);
            var roleClaims = new List<RoleClaimDto>();
            foreach(var rc in roleClaimValues)
            {
                try
                {
                   roleClaims = rc.FromJson<List<string>>().Select(s => s.FromJson<RoleClaimDto>()).ToList();
                }
                catch
                {
                    roleClaims.Add(rc.FromJson<RoleClaimDto>());
                }
            }

            return new UserClaimDto
            {
                UserId = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty,
                Email = email ?? string.Empty,
                RoleClaims = roleClaims,
                IsAuthenticated = true
            };
        }
    }
}