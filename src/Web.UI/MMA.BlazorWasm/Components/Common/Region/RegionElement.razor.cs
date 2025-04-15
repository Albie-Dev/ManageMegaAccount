using Microsoft.AspNetCore.Components;
using MMA.Domain;

namespace MMA.BlazorWasm.Components.Common.Region
{
    public partial class RegionElement
    {
        [Parameter]
        public CRegionType RegionType { get; set; }

        private string GetFlagUrl()
        {
            return RegionType switch
            {
                CRegionType.Afghanistan => "https://flagcdn.com/w40/af.png",
                CRegionType.Armenia => "https://flagcdn.com/w40/am.png",
                CRegionType.Azerbaijan => "https://flagcdn.com/w40/az.png",
                CRegionType.Bahrain => "https://flagcdn.com/w40/bh.png",
                CRegionType.Bangladesh => "https://flagcdn.com/w40/bd.png",
                CRegionType.Bhutan => "https://flagcdn.com/w40/bt.png",
                CRegionType.Brunei => "https://flagcdn.com/w40/bn.png",


                CRegionType.Japan => "https://flagcdn.com/w40/jp.png",


                CRegionType.Vietnam => "https://flagcdn.com/w40/bn.png",



                _ => "https://flagcdn.com/w40/vn.png"
            };
        }
    }
}