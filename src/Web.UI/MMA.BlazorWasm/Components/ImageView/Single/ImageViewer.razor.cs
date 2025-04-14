using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MMA.BlazorWasm.Components.ImageView.Single
{
    public partial class ImageViewer
    {
        [Parameter] public string ImageUrl { get; set; } = string.Empty;
        [Parameter] public bool IsOpen { get; set; }
        [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

        private void CloseImageViewer()
        {
            IsOpen = false;
            IsOpenChanged.InvokeAsync(IsOpen);
        }

        void HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                IsOpen = false;
                IsOpenChanged.InvokeAsync(IsOpen);
            }
        }
    }
}