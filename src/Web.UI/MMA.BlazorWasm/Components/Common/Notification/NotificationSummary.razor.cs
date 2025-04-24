using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Notification
{
    public partial class NotificationSummary
    {
        [Parameter] public NotificationResponse NotificationResponse { get; set; } = null!;

        private string GetNotificationStyle()
        {
            return NotificationResponse.Level switch
            {
                CNotificationLevel.Success => "background-color: green; color: white;",
                CNotificationLevel.Info => "background-color: blue; color: white;",
                CNotificationLevel.Warning => "background-color: orange; color: black;",
                CNotificationLevel.Error => "background-color: red; color: white;",
                _ => "background-color: gray; color: white;",
            };
        }
    }
}