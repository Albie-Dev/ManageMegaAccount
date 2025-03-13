namespace MMA.Domain
{
    public class DropdownItemModel
    {
        public string Name { get; set; } = string.Empty;
        public object Value { get; set; } = null!;
        public bool IsSelected { get; set; } = false;
    }
}