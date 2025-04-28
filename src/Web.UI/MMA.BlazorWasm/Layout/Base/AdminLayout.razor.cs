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
                GroupName = "Role Management",
                Items = new List<SidebarItemModel>()
                {
                    new SidebarItemModel()
                    {
                        Icon = "bi bi-speedometer2",
                        Route = "/admin/role",
                        Title = "Roles",
                        CollapseId = "roleCollapse",
                    //     SubItems = new List<SidebarItemModel>()
                    //     {
                    //         new SidebarItemModel()
                    //         {
                    //             Icon = "bi bi-badge-ar",
                    //             Title = "Default",
                    //             Route = "/admin/role",
                    //         },
                    //         new SidebarItemModel()
                    //         {
                    //             Icon = "bi bi-badge-ar",
                    //             Title = "Multipurpose",
                    //             Route = "/haibuoc",
                    //         },
                    //         new SidebarItemModel()
                    //         {
                    //             Icon = "bi bi-badge-ar",
                    //             Title = "Affiliate",
                    //             Route = "/haibuoc",
                    //         }
                    //     }
                    }
                }
            },

            new MenuSideModel()
            {
                GroupName = "Actor Management",
                Items = new List<SidebarItemModel>()
                {
                    new SidebarItemModel()
                    {
                        Icon = "bi bi-person-video2",
                        Route = "/movie/actor",
                        Title = "Actors",
                        CollapseId = "actorCollapse",
                    //     SubItems = new List<SidebarItemModel>()
                    //     {
                    //         new SidebarItemModel()
                    //         {
                    //             Icon = "bi bi-badge-ar",
                    //             Title = "Default",
                    //             Route = "/admin/role",
                    //         },
                    //         new SidebarItemModel()
                    //         {
                    //             Icon = "bi bi-badge-ar",
                    //             Title = "Multipurpose",
                    //             Route = "/haibuoc",
                    //         },
                    //         new SidebarItemModel()
                    //         {
                    //             Icon = "bi bi-badge-ar",
                    //             Title = "Affiliate",
                    //             Route = "/haibuoc",
                    //         }
                    //     }
                    }
                }
            }
        };
    }
}