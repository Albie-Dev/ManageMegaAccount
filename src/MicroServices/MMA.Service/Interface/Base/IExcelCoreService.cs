using MMA.Domain;

namespace MMA.Service
{
    public interface IExcelCoreService
    {
        Task<List<T>> ImportExcelByTemplateAsync<T>(
            Stream excelStream,
            string sheetKey,
            Dictionary<string, string> columnTitles,
            Dictionary<Type, Dictionary<string, object>>? translation = null
        ) where T : new();
        Task<ImportResult<object>> ImportExcelByTemplateWithMultipleSheetAsync(
            Stream excelStream,
            Dictionary<string, (Type DtoType, Dictionary<string, string> ColumnTitles)> sheetConfigs,
            Dictionary<Type, Dictionary<string, object>>? translations = null,
            bool validateData = true);
        Task<byte[]> ExportExcelByTemplateAsync<T>(IEnumerable<T> exportDataModels, string fileName,
            string sheetKey, string sheetName);
        Task<byte[]> ExportExcelMultipleSheetsAsync(
            List<SheetExportInfo> sheetExports,
            string templateFileName);
    }
}