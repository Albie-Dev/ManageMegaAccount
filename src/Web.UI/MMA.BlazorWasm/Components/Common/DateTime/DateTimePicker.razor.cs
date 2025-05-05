using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.DateTime
{
    public partial class DateTimePicker
    {
        [Parameter] public string LabelName { get; set; } = string.Empty;
        [Parameter] public CDateTimePickerType Type { get; set; }
        [Parameter] public CWidthType Width { get; set; } = CWidthType.Col12;
        [Parameter] public DateTimeRageFilterProperty<DateTimeOffset> DateTimeRange { get; set; } = new DateTimeRageFilterProperty<DateTimeOffset>();
        [Parameter] public EventCallback<DateTimeRageFilterProperty<DateTimeOffset>> DateTimeRangeChanged { get; set; }
        [Parameter] public DateTimeRageFilterProperty<DateOnly> DateRange { get; set; } = new DateTimeRageFilterProperty<DateOnly>();
        [Parameter] public EventCallback<DateTimeRageFilterProperty<DateOnly>> DateRangeChanged { get; set; }
        [Parameter] public DateTimeRageFilterProperty<TimeOnly> TimeRange { get; set; } = new DateTimeRageFilterProperty<TimeOnly>();
        [Parameter] public EventCallback<DateTimeRageFilterProperty<TimeOnly>> TimeRangeChanged { get; set; }
        [Parameter] public string ParentElementClassName { get; set; } = string.Empty;

        private string _inputId { get; set; } = $"InputID{Guid.NewGuid()}";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _jsRuntime.InvokeVoidAsync("initDateTimePicker", Type.ToString(), null, null,
                ParentElementClassName, _inputId, DotNetObjectReference.Create(this));
            }
        }

        [JSInvokable]
        public async Task UpdateDate(string start, string end)
        {
            var format = "dd/MM/yyyy";
            var culture = CultureInfo.InvariantCulture;
            DateRange.Start = DateOnly.ParseExact(start, format, culture);
            DateRange.End = DateOnly.ParseExact(end, format, culture);
            System.Console.WriteLine($"Start = {DateRange.Start} - End : {DateRange.End}");
            await DateRangeChanged.InvokeAsync(DateRange);
            StateHasChanged();
        }

        [JSInvokable]
        public async Task UpdateTime(string start, string end)
        {
            var format = "HH:mm:ss";
            var culture = CultureInfo.InvariantCulture;
            TimeRange.Start = TimeOnly.ParseExact(start, format, culture);
            TimeRange.End = TimeOnly.ParseExact(end, format, culture);
            await TimeRangeChanged.InvokeAsync(TimeRange);
            StateHasChanged();
        }

        [JSInvokable]
        public async Task UpdateDateTime(string start, string end)
        {
            var format = "dd/MM/yyyy HH:mm";
            var culture = CultureInfo.InvariantCulture;
            DateTimeRange.Start = DateTimeOffset.ParseExact(start, format, culture);
            DateTimeRange.End = DateTimeOffset.ParseExact(end, format, culture);
            await DateTimeRangeChanged.InvokeAsync(DateTimeRange);
            StateHasChanged();
        }

        private async Task ClearDate()
        {
            DateRange.Start = null;
            DateRange.End = null;
            await DateRangeChanged.InvokeAsync(DateRange);
            await _jsRuntime.InvokeVoidAsync("clearDatePicker", _inputId);
            StateHasChanged();
        }

        private async Task ClearTime()
        {
            TimeRange.Start = null;
            TimeRange.End = null;
            await TimeRangeChanged.InvokeAsync(TimeRange);
            await _jsRuntime.InvokeVoidAsync("clearTimePicker", _inputId);
            StateHasChanged();
        }

        private async Task ClearDateTime()
        {
            DateTimeRange.Start = null;
            DateTimeRange.End = null;
            await DateTimeRangeChanged.InvokeAsync(DateTimeRange);
            await _jsRuntime.InvokeVoidAsync("clearDateTimePicker", _inputId);
            StateHasChanged();
        }
    }
}