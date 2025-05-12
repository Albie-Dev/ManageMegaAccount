using Microsoft.AspNetCore.Components;

namespace MMA.BlazorWasm.Components.Common.RateStar
{
    public partial class RateStarElement
    {
        [Parameter]
        public double RateStar { get; set; }

        [Parameter]
        public EventCallback<double> RateStarChanged { get; set; }

        [Parameter]
        public string StarSize { get; set; } = "1.5rem";

        [Parameter]
        public string FontSize { get; set; } = "1rem";

        private double HighlightValue { get; set; }

        private async Task UpdateRating(double value)
        {
            RateStar = value;
            HighlightValue = value;
            await RateStarChanged.InvokeAsync(RateStar);
        }

        private void HighlightStars(double value)
        {
            HighlightValue = value;
        }

        private string GetStarClass(int starValue)
        {
            double displayValue = HighlightValue > 0 ? HighlightValue : RateStar;
            if (starValue <= Math.Floor(displayValue))
            {
                return "bi bi-star-fill";
            }
            else if (starValue == Math.Ceiling(displayValue) && displayValue % 1 > 0)
            {
                return "bi bi-star-half";
            }
            else
            {
                return "bi bi-star empty";
            }
        }
    }
}