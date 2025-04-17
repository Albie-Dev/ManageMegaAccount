namespace MMA.Service
{
    public interface IExcelCoreService
    {
        Task<byte[]> ExportExcelByTemplateAsync<T>(IEnumerable<T> exportDataModels, string fileName,
            string assemblyName, string sheetKey, string sheetName);
    }
}