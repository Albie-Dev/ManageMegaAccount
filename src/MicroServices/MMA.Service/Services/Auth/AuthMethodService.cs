using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MMA.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MMA.Service
{
    public class AuthMethodService : IAuthMethodService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbRepository _repository;

        public AuthMethodService(
            IDbRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        // Generate JWT Token
        public async Task<string> GenerateAccessTokenAsync(UserEntity user)
        {
            var jwtConfig = RuntimeContext.AppSettings.JwtConfig;
            var userRoles = await _repository.Queryable<UserRoleEntity>().Include(sr => sr.Role)
                .Where(sr => sr.UserId == user.Id).ToListAsync();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, (new RoleClaimDto
                {
                    RoleName = userRole.Role.RoleName,
                    RolePermissions = userRole.RolePermissions
                }).ToJson()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwtConfig.Issuer,
                jwtConfig.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(UserEntity user)
        {
            var jwtConfig = RuntimeContext.AppSettings.JwtConfig;

            var refreshTokenClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("TokenType", "RefreshToken")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtRefreshToken = new JwtSecurityToken(
                jwtConfig.Issuer,
                jwtConfig.Audience,
                refreshTokenClaims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken);
            return await Task.FromResult<string>(refreshToken);
        }

        // Generate Authentication Cookie
        public void SetAuthCookie(HttpContext context, string token)
        {
            context.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SameSite = SameSiteMode.Strict
            });
        }

        // Manage Session
        public void SetUserSession(HttpContext context, string key, string value)
        {
            context.Session.SetString(key, value);
        }

        public string GetUserSession(HttpContext context, string key)
        {
            return context.Session.GetString(key) ?? string.Empty;
        }

        // JWT Bearer Authentication
        public void ConfigureJwtAuthentication(IServiceCollection services, string issuer, string audience, string signingKey)
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
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                    };
                });
        }

        // Cookie Authentication
        public void ConfigureCookieAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = "/auth/login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    options.SlidingExpiration = true;
                });
        }

        // Session Management
        public void ConfigureSession(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // Validate User Permissions
        public async Task<bool> HasPermission(CResourceType resourceType, CPermissionType permissionType)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || (user.Identity != null && !user.Identity.IsAuthenticated))
                return false;

            var rolePermissions = await GetUserRolePermissions(user);

            return rolePermissions.Any(rp =>
                rp.ResourceType == resourceType &&
                rp.PermissionTypes.Contains(permissionType));
        }

        private async Task<List<RolePermission>> GetUserRolePermissions(ClaimsPrincipal user)
        {
            // Assuming roles and permissions are stored in claims.
            Guid.TryParse(user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .FirstOrDefault(), out Guid userId);
            var permissions = new List<RolePermission>();

            // Simulated logic to fetch role permissions from database.
            var userRoles = await _repository.GetAsync<UserRoleEntity>(s => s.UserId == userId);
            return userRoles.SelectMany(s => s.RolePermissions).ToList();
        }
    }
}
