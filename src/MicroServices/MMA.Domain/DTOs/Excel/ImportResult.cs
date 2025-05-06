namespace MMA.Domain
{
    public class ImportResult<T> where T : new()
    {
        /// <summary>
        /// Key : this is sheet name
        /// Value : List<DTOs> data model of sheet
        /// </summary>
        public Dictionary<string, List<ImportRow<T>>> ResultDics { get; set; } = new Dictionary<string, List<ImportRow<T>>>();
        public byte[] FileResult { get; set; } = new byte[0];
        public bool Result { get; set; } = true;
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