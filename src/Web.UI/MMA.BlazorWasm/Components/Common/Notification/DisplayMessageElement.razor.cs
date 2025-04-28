using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Notification
{
    public partial class DisplayMessageElement
    {
        [Parameter] public NotificationResponse? Notification { get; set; }

        private string GetNotificationClass(CNotificationLevel level)
        {
            return level switch
            {
                CNotificationLevel.Success => "alert-success",
                CNotificationLevel.Info => "alert-info",
                CNotificationLevel.Warning => "alert-warning",
                CNotificationLevel.Error => "alert-danger",
                _ => "alert-secondary"
            };
        }

        private string GetIconClass(CNotificationLevel level)
        {
            return level switch
            {
                CNotificationLevel.Success => "bi bi-check-circle-fill text-success",
                CNotificationLevel.Info => "bi bi-info-circle-fill text-info",
                CNotificationLevel.Warning => "bi bi-exclamation-triangle-fill text-warning",
                CNotificationLevel.Error => "bi bi-x-circle-fill text-danger",
                _ => "bi bi-info-circle-fill text-secondary"
            };
        }
    }
}