using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!string.IsNullOrWhiteSpace(policyName))
        {
            var parts = policyName.Split('.');
            var policyBuilder = new AuthorizationPolicyBuilder();

            if (parts.Length >= 1)
            {
                policyBuilder.RequireClaim("Role", parts[0]);
            }

            if (parts.Length >= 2)
            {
                policyBuilder.RequireClaim("Resource", parts[1]);
            }

            if (parts.Length >= 3)
            {
                policyBuilder.RequireClaim("Permission", parts[2]);
            }

            return Task.FromResult<AuthorizationPolicy?>(policyBuilder.Build());
        }
        return base.GetPolicyAsync(policyName);
    }
}
