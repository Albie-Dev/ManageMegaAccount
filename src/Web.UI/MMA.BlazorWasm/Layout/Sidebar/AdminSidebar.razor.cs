using Microsoft.AspNetCore.Components;

namespace MMA.BlazorWasm.Layout.Sidebar
{
    public partial class AdminSidebar
    {
        [Parameter]
        public List<MenuSideModel> Routes { get; set; } = new List<MenuSideModel>();
    }
}