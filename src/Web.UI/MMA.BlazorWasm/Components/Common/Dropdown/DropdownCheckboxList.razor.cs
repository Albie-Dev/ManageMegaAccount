using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Dropdown
{
    public partial class DropdownCheckboxList
    {
        [Parameter] public string Class { get; set; } = string.Empty;
        [Parameter] public string Title { get; set; } = "Choose Options";
        [Parameter] public List<DropdownItemModel> Items { get; set; } = new List<DropdownItemModel>();
        [Parameter] public EventCallback<List<DropdownItemModel>> OnApply { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private bool selectAll = false;
        private bool isOpen = false;
        private string searchTerm = string.Empty;

        private List<DropdownItemModel> FilteredItems => Items
            .Where(item => item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        private string DropdownClass => isOpen ? "dropdown-menu show" : "dropdown-menu";

        private void OnSearchInput(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? string.Empty;
            StateHasChanged();
        }

        private void ToggleSelectAll()
        {
            selectAll = !selectAll;
            foreach (var item in Items)
            {
                item.IsSelected = selectAll;
            }
        }

        private void ToggleItemSelection(DropdownItemModel item)
        {
            item.IsSelected = !item.IsSelected;
            selectAll = Items.All(i => i.IsSelected);
        }

        private void ResetSelection()
        {
            foreach (var item in Items)
            {
                item.IsSelected = false;
            }
            selectAll = false;
        }

        private void CancelSelection()
        {
            isOpen = false;
            OnCancel.InvokeAsync();
        }

        private void ApplySelection()
        {
            isOpen = false;
            OnApply.InvokeAsync(Items.Where(item => item.IsSelected).ToList());
        }
    }
}