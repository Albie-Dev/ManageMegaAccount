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
        private List<string> FormSummaryErrors { get; set; } = new();

        protected override void OnAfterRender(bool firstRender)
        {
            HandleErrors();
        }

        private void HandleErrors()
        {
            if (Errors.IsNullOrEmpty()) return;

            foreach (var error in Errors)
            {
                switch (error.ErrorScope)
                {
                    case CErrorScope.Field:
                        // Display field-specific error
                        if ($"{FieldName}_Error" == error.Field)
                        {
                            ErrorMessage = error.Error;
                        }
                        break;
                    case CErrorScope.FormSummary:
                        // Display form summary error
                        FormSummaryErrors.Add(item: error.Error);
                        break;
                    case CErrorScope.PageSumarry:
                        // Display page summary error using toast service
                        _toastService.ShowError(error.Error);
                        break;
                    case CErrorScope.RedirectPage:
                        // Redirect to notification summary page
                        _navigationManager.NavigateTo("/notificationsummary");
                        break;
                    case CErrorScope.Global:
                        // Redirect to internal server error page
                        _navigationManager.NavigateTo("/internalservererror");
                        break;
                    case CErrorScope.RedirectToLoginPage:
                        // Redirect to login page
                        _navigationManager.NavigateTo("/login");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}