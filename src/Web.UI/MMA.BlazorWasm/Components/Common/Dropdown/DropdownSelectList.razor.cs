using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Dropdown
{
    public partial class DropdownSelectList
    {
        private string _idDataList { get; set; } = $"{Guid.NewGuid()}_DataListId";
        [Parameter] public string Title { get; set; } = "Choose Option";
        [Parameter] public List<DropdownItemModel> Items { get; set; } = new List<DropdownItemModel>();
        [Parameter] public EventCallback<object> BindDataChanged { get; set; }

        private string searchTerm = string.Empty;
        private DropdownItemModel? selectedItem { get; set; }

        private List<DropdownItemModel> FilteredItems => string.IsNullOrWhiteSpace(searchTerm) ?
            Items : Items.Where(item => item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        private void OnInputChanged(ChangeEventArgs e)
        {
            string selectedName = e.Value?.ToString() ?? string.Empty;
            selectedItem = Items.FirstOrDefault(item => item.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase));

            if (selectedItem != null)
            {
                BindDataChanged.InvokeAsync(selectedItem.Value);
                StateHasChanged();
            }
        }
    }
}