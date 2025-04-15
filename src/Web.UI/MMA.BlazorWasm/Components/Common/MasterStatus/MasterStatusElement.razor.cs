using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.MasterStatus
{
    public partial class MasterStatusElement
    {
        [Parameter]
        public CMasterStatus Status { get; set; }

        private string GetStatusColor()
        {
            return Status switch
            {
                CMasterStatus.Active => "#0dcaf0", // Cyan-500
                CMasterStatus.Deactive => "#d4b106", // Yellow-700
                CMasterStatus.None => "#6c757d", // Gray-500
                _ => "#000000"
            };
        }
    }
}