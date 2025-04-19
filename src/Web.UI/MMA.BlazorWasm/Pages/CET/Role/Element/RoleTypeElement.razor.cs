using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Pages.CET.Role.Element
{
    public partial class RoleTypeElement
    {
        [Parameter]
        public CRoleType RoleType { get; set; }

        private string GetRoleTypeColor()
        {
            return RoleType switch
            {
                CRoleType.Admin => "#0d6efd",      // Blue
                CRoleType.Client => "#198754",     // Green
                CRoleType.None => "#6c757d",       // Gray
                _ => "#000000"
            };
        }
    }
}