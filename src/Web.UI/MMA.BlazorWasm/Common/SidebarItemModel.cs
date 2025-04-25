namespace MMA.BlazorWasm
{
    public class SidebarItemModel
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public List<SidebarItemModel> SubItems = new List<SidebarItemModel>();
    }
}