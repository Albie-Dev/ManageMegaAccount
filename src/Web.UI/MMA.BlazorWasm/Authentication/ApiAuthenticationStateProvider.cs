using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using MMA.Domain;
using Microsoft.AspNetCore.Components.Authorization;

namespace MMA.BlazorWasm
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly PermissionService _permissionService;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, PermissionService permissionService)
        {
            _localStorage = localStorage;
            _permissionService = permissionService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync(key: ApiClientConstant.LocalStorage_Key);
            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var loginResponseDto = token.FromJson<LoginResponseDto>();
            if (loginResponseDto == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(loginResponseDto.AccessToken), "jwt");
            var user = new ClaimsPrincipal(identity);
            _permissionService.SetUser(user);
            return new AuthenticationState(user);
        }

        public void MarkUserAsAuthenticated(LoginResponseDto tokenData)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(tokenData.AccessToken), "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            foreach (var kvp in keyValuePairs ?? new())
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }

}