using MMA.Domain;

namespace MMA.Service
{
    public interface IExcelCoreService
    {
        Task<List<T>> ImportExcelByTemplateAsync<T>(
            Stream excelStream,
            string sheetKey,
            Dictionary<string, string> columnTitles,
            Dictionary<Type, Dictionary<string, object>>? translatedEnumValueMaps = null
        ) where T : new();
        Task<(Dictionary<string, ImportResult<object>>, byte[])> ImportExcelByTemplateAsync(
            Stream excelStream,
            Dictionary<string, (Type DtoType, Dictionary<string, string> ColumnTitles)> sheetConfigs,
            Dictionary<Type, Dictionary<string, object>>? translatedEnumValueMaps = null,
            bool validateData = true);
        Task<byte[]> ExportExcelByTemplateAsync<T>(IEnumerable<T> exportDataModels, string fileName,
            string assemblyName, string sheetKey, string sheetName);
    }
}