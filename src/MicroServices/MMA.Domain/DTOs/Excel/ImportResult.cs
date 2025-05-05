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

    public enum CImportResultType
    {
        None = 0,
        Success = 1,
        Failed = 2
    }
}