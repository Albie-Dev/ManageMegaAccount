namespace MMA.BlazorWasm
{
    public class SidebarItemModel
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string CollapseId { get; set; } = string.Empty;
        public List<SidebarItemModel> SubItems = new List<SidebarItemModel>();
    }

    public class MenuSideModel
    {
        public string GroupName { get; set; } = string.Empty;
        public List<SidebarItemModel> Items { get; set; } = new List<SidebarItemModel>();
    }
}