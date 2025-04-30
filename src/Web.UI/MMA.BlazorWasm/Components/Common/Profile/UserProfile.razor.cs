using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Profile
{
    public partial class UserProfile
    {
        [Parameter]
        public UserBaseInfoDto UserInfo { get; set; } = new();

        [Parameter]
        public int Size { get; set; } = 50;

        [Parameter]
        public bool ShowFullName { get; set; } = false;

        private string _tooltipDisplay = "none";

        private void ShowTooltip()
        {
            _tooltipDisplay = "block";
        }

        private void HideTooltip()
        {
            _tooltipDisplay = "none";
        }

        private void NavigateToUserDetail()
        {
            _navigationManager.NavigateTo($"/userdetail/{UserInfo.UserId}");
        }

        private string GetInitial(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var initial = names[0][0].ToString();
            if (names.Length > 1)
            {
                initial += names[^1][0];
            }
            return initial.ToUpper();
        }
    }
}