using Microsoft.AspNetCore.Components;

namespace MMA.BlazorWasm.Layout.Sidebar
{
    public partial class AdminSidebar
    {
        private bool _isToggle { get; set; } = true;
        [Parameter]
        public List<MenuSideModel> Routes { get; set; } = new List<MenuSideModel>();

        private void ToggleSidebar()
        {
            _isToggle = !_isToggle;
        }
    }
}