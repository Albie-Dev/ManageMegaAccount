/*
    This file create by Albie-Dev on 09/03/2025 3:45 PM
*/
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CG.Web.MegaApiClient;
using Imagekit;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MMA.Service
{
    /// <summary>
    /// This static class contains an extension method for adding MMA services to the 
    /// dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Extension method to register MMA services in the DI container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The updated IServiceCollection with MMA services registered.</returns>
        public static IServiceCollection AddMMAService(this IServiceCollection services)
        {
            // Register MMA services here
            services.AddHttpContextAccessor();
            services.AddDbContext<MMADbContext>(context =>
            {
                context.UseSqlServer(
                    RuntimeContext.AppSettings.ConnectionStrings.Core,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )
                );
            });

            services.AddScoped<IDbRepository, DbRepository>();


            services.AddScoped<IAuthMethodService, AuthMethodService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMegaApiClient, MegaApiClient>();
            services.AddScoped<IMegaService, MegaService>();
            services.AddSingleton<ImagekitClient>(provider => 
            {
                var logger = provider.GetRequiredService<ILogger<ImagekitClient>>();
                logger.LogInformation($"Start create new instance of ImageKit");
                return new ImagekitClient(
                    publicKey: RuntimeContext.AppSettings.CloudSetting.ImageKitIOConfig.PublicKey,
                    privateKey: RuntimeContext.AppSettings.CloudSetting.ImageKitIOConfig.PrivateKey,
                    urlEndPoint: RuntimeContext.AppSettings.CloudSetting.ImageKitIOConfig.UrlEndpoint
                );
            });
            services.AddScoped<IImageKitIOService, ImageKitIOService>();


            services.AddScoped<IActorService, ActorService>();
            return services;
        }




        public static IApplicationBuilder MapAccesTokenFromQuery(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var token = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                        if (jwtToken != null && jwtToken.Header.Count > 0)
                        {
                            context.Request.Headers["Authorization"] = $"Bearer {token}";
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync("Invalid or malformed token.");
                            return;
                        }
                    }
                    catch
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid or malformed token.");
                        return;
                    }
                }

                await next();
            });

            return app;
        }

        public static IApplicationBuilder MapCoreMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
            return app;
        }

        public static IServiceCollection AddCoreService(this IServiceCollection services)
        {
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        public static IServiceCollection AddCoreAuthentication<T>(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidIssuer = RuntimeContext.AppSettings.JwtConfig.Issuer,
                    ValidAudience = RuntimeContext.AppSettings.JwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RuntimeContext.AppSettings.JwtConfig.SecretKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
                        logger.LogWarning("Authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
