using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Dropdown
{
    public partial class DropdownElement
    {
        private Guid _dropDownId { get; set; } = Guid.NewGuid();

        [Parameter]
        public string LabelName { get; set; } = string.Empty;

        [Parameter]
        public List<DropdownItemModel> Items { get; set; } = new();

        [Parameter]
        public DropdownItemModel? SelectedOption { get; set; }

        [Parameter]
        public EventCallback<DropdownItemModel> SelectedOptionChanged { get; set; }

        private bool IsOpen { get; set; }
        private string SearchText { get; set; } = string.Empty;

        private IEnumerable<DropdownItemModel> FilteredOptions =>
            string.IsNullOrWhiteSpace(SearchText)
                ? Items
                : Items.Where(o => o.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        private void ToggleDropdown()
        {
            IsOpen = !IsOpen;
            if (IsOpen) SearchText = string.Empty;
        }

        private void SelectOption(DropdownItemModel option)
        {
            SelectedOption = option;
            SelectedOptionChanged.InvokeAsync(option);
            IsOpen = false;
        }

        protected override void OnInitialized()
        {
            if (Items == null)
            {
                Items = new List<DropdownItemModel>();
            }
        }

        // Close dropdown when clicking outside
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _jsRuntime.InvokeVoidAsync("setupDropdownCloseHandler",
                    DotNetObjectReference.Create(this));
            }
        }

        [JSInvokable]
        public void CloseDropdown()
        {
            IsOpen = false;
            StateHasChanged();
        }
    }
}