using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Layout.Auth
{
    public partial class AuthLayout
    {
        [Inject] private ApiAuthenticationStateProvider _authProvider { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await _authProvider.GetAuthenticationStateAsync();
            var currentUser = authenticationState.User.ToUserClaim();
            if (currentUser != null && currentUser.IsAuthenticated)
            {
                _navigationManager.NavigateTo("/");
            }
        }
    }
}