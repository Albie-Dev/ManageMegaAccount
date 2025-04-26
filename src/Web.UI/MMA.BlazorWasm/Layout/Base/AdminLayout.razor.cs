using Microsoft.JSInterop;

namespace MMA.BlazorWasm.Layout.Base
{
    public partial class AdminLayout
    {

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _jsRuntime.InvokeVoidAsync("sidebarManager.init");
            }
        }

        public List<MenuSideModel> Routes => new List<MenuSideModel>()
        {
            new MenuSideModel()
            {
                GroupName = "Core",
                Items = new List<SidebarItemModel>()
                {
                    new SidebarItemModel()
                    {
                        Icon = "bi bi-speedometer2",
                        Route = "/haibuoc",
                        Title = "Dashboards",
                        CollapseId = "dashCollapse",
                        SubItems = new List<SidebarItemModel>()
                        {
                            new SidebarItemModel()
                            {
                                Icon = "",
                                Title = "Default",
                                Route = "/haibuoc/test",
                            },
                            new SidebarItemModel()
                            {
                                Icon = "",
                                Title = "Multipurpose",
                                Route = "/haibuoc",
                            },
                            new SidebarItemModel()
                            {
                                Icon = "",
                                Title = "Affiliate",
                                Route = "/haibuoc",
                            }
                        }
                    }
                }
            }
        };
    }
}