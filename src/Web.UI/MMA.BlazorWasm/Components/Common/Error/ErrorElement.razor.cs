using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Error
{
    public partial class ErrorElement
    {
        [Parameter]
        public List<ErrorDetailDto> Errors { get; set; } = new List<ErrorDetailDto>();

        [Parameter]
        public string FieldName { get; set; } = string.Empty;

        [Parameter]
        public string FormSummaryId { get; set; } = string.Empty;

        private string ErrorMessage { get; set; } = string.Empty;
        private List<string> FormSummaryErrors { get; set; } = new List<string>();

        protected override async Task OnParametersSetAsync()
        {
            await HandleErrorsAsync();
        }

        private async Task HandleErrorsAsync()
        {
            ErrorMessage = string.Empty;
            FormSummaryErrors.Clear();

            if (Errors == null || !Errors.Any()) return;

            var fieldErrors = Errors.Where(e => e.ErrorScope == CErrorScope.Field && $"{FieldName}_Error" == e.Field).Select(e => e.Error).ToList();

            if (fieldErrors.Any())
            {
                ErrorMessage = string.Join("\n", fieldErrors);
            }

            var formSummaryErrors = Errors.Where(e => e.ErrorScope == CErrorScope.FormSummary).Select(e => e.Error).ToList();

            if (formSummaryErrors.Any())
            {
                FormSummaryErrors.AddRange(formSummaryErrors);
            }

            await InvokeAsync(StateHasChanged);

            await Task.Delay(8000);

            ErrorMessage = string.Empty;
            FormSummaryErrors.Clear();

            await InvokeAsync(StateHasChanged);
        }
    }
}