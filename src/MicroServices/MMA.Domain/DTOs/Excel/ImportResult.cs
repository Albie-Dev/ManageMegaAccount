namespace MMA.Domain
{
    public class ImportResult<T> where T : new()
    {
        public string SheetName { get; set; } = string.Empty;
        public List<ImportRow<T>> Rows { get; set; } = new List<ImportRow<T>>();
    }

    public class ImportRow<T> where T : new()
    {
        public T? Data { get; set; }
        public CImportResultType Result { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class SheetExportInfo
    {
        public IEnumerable<object>? Data { get; set; }
        public Type ModelType { get; set; } = default!;
        public string SheetKey { get; set; } = string.Empty;
        public string SheetName { get; set; } = string.Empty;
        public Dictionary<string, string> ColumnTitles { get; set; } = new();
        public Dictionary<Type, Dictionary<string, object>> Translations { get; set; } = new();
        public Dictionary<string, Func<object?, string>> CustomFormatters { get; set; } = new();
    }


    public enum CImportResultType
    {
        None = 0,
        Success = 1,
        Failed = 2
    }
}