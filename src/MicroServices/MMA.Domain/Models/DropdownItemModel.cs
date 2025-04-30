namespace MMA.Domain
{
    public class DropdownItemModel
    {
        public string Name { get; set; } = string.Empty;
        public object Value { get; set; } = null!;
        public bool IsSort { get; set; } = false;
        public bool IsSelected { get; set; } = false;
        public string TdStyle { get; set; } = string.Empty;
        public string TdClass { get; set; } = string.Empty;
    }
}