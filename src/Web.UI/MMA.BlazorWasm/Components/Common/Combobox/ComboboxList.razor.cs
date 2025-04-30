
using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Combobox
{
    public partial class ComboboxList<T>
    {
        [Parameter] public string ClassName { get; set; } = string.Empty;
        [Parameter] public string PlaceHolderName { get; set; } = string.Empty;
        [Parameter]
        public string TagStyle { get; set; } = string.Empty;
        [Parameter]
        public string TagClass { get; set; } = "bg-primary text-white";
        [Parameter] public CComboboxModeType Mode { get; set; } = CComboboxModeType.Select;

        [Parameter] public List<CComboboxModel> Items { get; set; } = new();

        [Parameter] public T? BindValue { get; set; }
        [Parameter] public EventCallback<T> BindValueChanged { get; set; }

        [Parameter] public List<T> BindValues { get; set; } = new();
        [Parameter] public EventCallback<List<T>> BindValuesChanged { get; set; }

        [Parameter] public EventCallback<T?> OnValueChanged { get; set; }
        [Parameter] public EventCallback<List<T>> OnValuesChanged { get; set; }


        public List<CComboboxModel> FilteredOptions { get; set; } = new();
        [Parameter]
        public List<CComboboxModel> SelectedOptions { get; set; } = new();
        public HashSet<T> SelectedValues { get; set; } = new();

        public bool ShowDropdown { get; set; }
        public string SearchFilter { get; set; } = string.Empty;
        public string RadioGroupName { get; set; } = $"RadioGroup_{Guid.NewGuid()}";

        protected override void OnInitialized()
        {
            FilteredOptions = Items.ToList();

            if (Mode == CComboboxModeType.Checkbox)
            {
                SelectedValues = new HashSet<T>(BindValues ?? []);
            }
            else if (BindValue != null)
            {
                SelectedValues = new HashSet<T> { BindValue };
            }

            SelectedOptions = Items.Where(i => i.Value != null && SelectedValues.Contains((T)i.Value)).ToList();
        }

        protected override void OnParametersSet()
        {
            if (Mode == CComboboxModeType.Checkbox)
            {
                SelectedValues = new HashSet<T>(BindValues ?? []);
            }
            else if (BindValue != null)
            {
                SelectedValues = new HashSet<T> { BindValue };
            }

            SelectedOptions = Items.Where(i => i.Value != null && SelectedValues.Contains((T)i.Value)).ToList();
        }

        public void ToggleDropdown() => ShowDropdown = !ShowDropdown;

        public async Task RemoveOption(CComboboxModel option)
        {
            if (Mode == CComboboxModeType.Checkbox)
            {
                var value = (T)option.Value!;
                SelectedValues.Remove(value);
                SelectedOptions = Items.Where(i => SelectedValues.Contains((T)i.Value!)).ToList();

                await BindValuesChanged.InvokeAsync(SelectedValues.ToList());
                await OnValuesChanged.InvokeAsync(SelectedValues.ToList());
            }
            else
            {
                SelectedValues.Clear();
                SelectedOptions.Clear();

                await BindValueChanged.InvokeAsync(default);
                await OnValueChanged.InvokeAsync(default);
            }

            ShowDropdown = false;
            StateHasChanged();
        }

        public async Task HandleRadioSelection(CComboboxModel option)
        {
            SelectedValues.Clear();
            SelectedValues.Add((T)option.Value!);
            SelectedOptions = new() { option };
            ShowDropdown = false;

            await BindValueChanged.InvokeAsync((T)option.Value!);
            await OnValueChanged.InvokeAsync((T)option.Value!);

            StateHasChanged();
        }

        public async Task HandleSelectOption(CComboboxModel option)
        {
            SelectedValues.Clear();
            SelectedValues.Add((T)option.Value!);
            SelectedOptions = new() { option };
            ShowDropdown = false;

            await BindValueChanged.InvokeAsync((T)option.Value!);
            await OnValueChanged.InvokeAsync((T)option.Value!);
            
            StateHasChanged();
        }

        public async Task HandleCheckboxSelection(CComboboxModel option, bool isChecked)
        {
            var value = (T)option.Value!;
            if (isChecked)
                SelectedValues.Add(value);
            else
                SelectedValues.Remove(value);

            SelectedOptions = Items.Where(i => SelectedValues.Contains((T)i.Value!)).ToList();
            await BindValuesChanged.InvokeAsync(SelectedValues.ToList());
            await OnValuesChanged.InvokeAsync(SelectedValues.ToList());
            StateHasChanged();
        }

        public void FilterOptions(ChangeEventArgs e)
        {
            SearchFilter = e.Value?.ToString() ?? "";
            FilteredOptions = string.IsNullOrWhiteSpace(SearchFilter)
                ? Items.ToList()
                : Items.Where(o => o.Name.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

}