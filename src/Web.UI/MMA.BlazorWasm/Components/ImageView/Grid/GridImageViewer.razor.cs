using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using MMA.Domain;


namespace MMA.BlazorWasm.Components.ImageView.Grid
{
    public partial class GridImageViewer
    {
        [Parameter]
        public List<ImageKitIOFileResponseDto?> Images { get; set; } = new List<ImageKitIOFileResponseDto?>();

        public bool IsModalOpen { get; set; }
        public int CurrentIndex { get; set; }

        public double ImageScale { get; set; } = 1.0;

        public string IndexInput { get; set; } = "";

        public ImageKitIOFileResponseDto? CurrentImage =>
            (Images != null && Images.Count > 0 && CurrentIndex >= 0 && CurrentIndex < Images.Count)
                ? Images[CurrentIndex]
                : null;

        private ElementReference modalKeyboardRef;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (IsModalOpen && !modalKeyboardRef.Equals(default(ElementReference)))
            {
                await modalKeyboardRef.FocusAsync();
            }
        }

        void OpenModal(int index)
        {
            CurrentIndex = index;
            ImageScale = 1.0;
            IndexInput = (index + 1).ToString();
            IsModalOpen = true;
        }

        void CloseModal() => IsModalOpen = false;

        void PrevImage()
        {
            if (Images != null && CurrentIndex > 0)
            {
                CurrentIndex--;
                ImageScale = 1.0;
                IndexInput = (CurrentIndex + 1).ToString();
            }
        }

        void NextImage()
        {
            if (Images != null && CurrentIndex < Images.Count - 1)
            {
                CurrentIndex++;
                ImageScale = 1.0;
                IndexInput = (CurrentIndex + 1).ToString();
            }
        }

        void ZoomIn()
        {
            ImageScale += 0.1;
        }

        void ZoomOut()
        {
            if (ImageScale > 0.2)
                ImageScale -= 0.1;
        }

        void HandleIndexInput(ChangeEventArgs e)
        {
            IndexInput = e.Value?.ToString() ?? "";
            if (int.TryParse(IndexInput, out int index))
            {
                if (index >= 1 && index <= Images.Count)
                {
                    CurrentIndex = index - 1;
                    ImageScale = 1.0;
                }
                else
                {
                    CurrentIndex = index > 1 ? Images.Count - 1 : 1;
                    ImageScale = 1.0;
                }
            }
        }

        void HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "ArrowRight")
            {
                NextImage();
            }
            else if (e.Key == "ArrowLeft")
            {
                PrevImage();
            }
            else if (e.Key == "Escape")
            {
                CloseModal();
            }
            else if (e.Key == "=" || e.Key == "+")
            {
                ZoomIn();
            }
            else if (e.Key == "-")
            {
                ZoomOut();
            }
        }
    }
}