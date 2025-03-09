using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MMA.Service
{
    public static class RuntimeContextMiddleware
    {
        public static IApplicationBuilder MapRuntimeContext(this IApplicationBuilder app)
        {
            RuntimeContext.ServiceProvider = app.ApplicationServices;
            // ILogger? logger = null;

            app.Use(async (context, next) =>
            {
                try
                {
                    // logger = context.RequestServices.GetRequiredService<ILogger>();

                    // Set current user and user ID in RuntimeContext
                    var dbContext = context.RequestServices.GetRequiredService<MMADbContext>();
                    var httpContextAccessor = context.RequestServices.GetRequiredService<IHttpContextAccessor>();

                    // Get the user ID from the claims
                    Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
                    RuntimeContext.IpAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;

                    // Get the current user and link helper from the database
                    var currentUser = await dbContext.Users.FindAsync(userId);
                    var linkHelper = await dbContext.Links.FirstOrDefaultAsync();
                    if (linkHelper != null)
                    {
                        RuntimeContext.LinkHelper = linkHelper;
                    }

                    // Set the current user and user ID in the RuntimeContext
                    if (userId != Guid.Empty)
                    {
                        RuntimeContext.CurrentUser = currentUser;
                        RuntimeContext.CurrentUserId = userId;
                    }

                    // Handle the authorization header or query parameter
                    var httpRequest = httpContextAccessor.HttpContext?.Request;
                    if (httpRequest != null && httpRequest.Headers.TryGetValue("Authorization", out var accessTokenValue) &&
                        accessTokenValue.ToString().StartsWith("Bearer "))
                    {
                        RuntimeContext.CurrentAccessToken = accessTokenValue.ToString().Substring("Bearer ".Length).Trim();
                    }
                    else
                    {
                        RuntimeContext.CurrentAccessToken = httpRequest?.Query["access_token"].FirstOrDefault() ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    // logger?.LogError(ex, "An error occurred while setting runtime context.");
                    System.Console.WriteLine(ex.Message);
                    // Optionally, you can also set RuntimeContext to some safe default values in case of failure
                    RuntimeContext.CurrentUser = null;
                    RuntimeContext.CurrentUserId = Guid.Empty;
                    RuntimeContext.CurrentAccessToken = string.Empty;
                    RuntimeContext.IpAddress = string.Empty;
                    RuntimeContext.LinkHelper = null;
                }
                await next.Invoke();
            });

            return app;
        }
    }
}