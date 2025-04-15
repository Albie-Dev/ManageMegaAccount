using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Combobox
{
    public partial class RadioComboboxList<T>
    {
        public string DropDownSearchId { get; set; } = $"{Guid.NewGuid()}_DropDownSearch";
        public string DropDownMainSearchId { get; set; } = $"{Guid.NewGuid()}_DropDownMainSearch";
        public string DropDownContentId { get; set; } = $"{Guid.NewGuid()}_DropdownContent";
        public string DropDownInputContainer { get; set; } = $"{Guid.NewGuid()}_DropdownInputContainer";
        public string RadioGroupName { get; set; } = $"RadioGroup_{Guid.NewGuid()}";
        [Parameter]
        public string ClassName { get; set; } = string.Empty;

        [Parameter]
        public List<CComboboxModel> Items { get; set; } = new();

        public CComboboxModel? SelectedOption { get; set; }

        [Parameter]
        public T? BindValue { get; set; }

        [Parameter]
        public EventCallback<T> BindValueChanged { get; set; }

        public List<CComboboxModel> FilteredOptions { get; set; } = new();
        public bool ShowDropdown { get; set; }
        public string SearchFilter { get; set; } = string.Empty;

        protected override void OnInitialized()
        {
            FilteredOptions = Items.ToList();

            if (BindValue != null && !string.IsNullOrWhiteSpace(BindValue.ToString()))
            {
                SelectedOption = Items.FirstOrDefault(o => (o.Value).Equals(BindValue));
            }
        }

        protected override void OnParametersSet()
        {
            if (BindValue != null && !string.IsNullOrWhiteSpace(BindValue.ToString()))
            {
                var option = Items.FirstOrDefault(o => o.Value.Equals(BindValue));
                if (option != null && option != SelectedOption)
                {
                    SelectedOption = option;
                }
            }
        }

        public void ToggleDropdown()
        {
            ShowDropdown = !ShowDropdown;
        }

        public async Task HandleRadioSelection(CComboboxModel option)
        {
            SelectedOption = option;
            ShowDropdown = false;
            await BindValueChanged.InvokeAsync((T)option.Value!);
            StateHasChanged();
        }

        public void FilterOptions(ChangeEventArgs e)
        {
            SearchFilter = e.Value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(SearchFilter))
            {
                FilteredOptions = Items.ToList();
            }
            else
            {
                var filter = SearchFilter.ToUpperInvariant();
                FilteredOptions = Items
                    .Where(o => o.Name.ToUpperInvariant().Contains(filter)
                        || (o.Value.ToString() ?? string.Empty).ToUpperInvariant().Contains(filter))
                    .ToList();
            }
        }
    }
}