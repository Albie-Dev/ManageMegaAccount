using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class ExcelCoreService : IExcelCoreService
    {
        private readonly ILogger<ExcelCoreService> _logger;

        public ExcelCoreService(
            ILogger<ExcelCoreService> logger
        )
        {
            _logger = logger;
        }

        public async Task<byte[]> ExportExcelByTemplateAsync<T>(IEnumerable<T> exportDataModels, string fileName,
            string assemblyName, string sheetKey, string sheetName)
        {
            using var stream = GetTemplateStream($".Templates.{fileName}");
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheet(sheetKey);

            if (sheet == null)
                throw new Exception($"Sheet '{sheetKey}' not found.");

            sheet.Name = sheetName;
            int titleRow = -1;
            int contentRow = -1;
            var props = typeof(T).GetProperties();

            for (int r = 1; r <= sheet.RowsUsed().Count(); r++)
            {
                var cellValue = sheet.Cell(r, 1).GetString();
                if (cellValue.StartsWith("{{Title."))
                    titleRow = r;
                if (cellValue.StartsWith("{{Content."))
                    contentRow = r;
            }

            if (titleRow == -1 || contentRow == -1)
                throw new Exception("Title or Content row not found.");

            for (int c = 1; c <= sheet.ColumnsUsed().Count(); c++)
            {
                var val = sheet.Cell(titleRow, c).GetString();
                var match = Regex.Match(val, @"\{\{Title\.([^\}]+)\}\}");
                if (match.Success)
                {
                    var title = match.Groups[1].Value;
                    sheet.Cell(titleRow, c).Value = title;
                }
            }

            int currentRow = contentRow + 1;
            foreach (var item in exportDataModels)
            {
                for (int c = 1; c <= sheet.ColumnsUsed().Count(); c++)
                {
                    var val = sheet.Cell(contentRow, c).GetString();
                    var match = Regex.Match(val, @"\{\{Content\.([^\}]+)\}\}");
                    if (match.Success)
                    {
                        var propName = match.Groups[1].Value;
                        var prop = props.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            var value = prop.GetValue(item);

                            if (value == null)
                            {
                                sheet.Cell(currentRow, c).Value = string.Empty;
                            }
                            else
                            {
                                sheet.Cell(currentRow, c).Value = value.ToString();
                            }
                        }
                    }
                }
                currentRow++;
            }
            sheet.Row(2).Delete();
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return await Task.FromResult<byte[]>(ms.ToArray());
        }

        public async Task<List<T>> ImportExcelByTemplateAsync<T>(
            Stream excelStream,
            string sheetKey,
            Dictionary<string, string> columnTitles,
            Dictionary<Type, Dictionary<string, object>>? translatedEnumValueMaps = null
        ) where T : new()
        {
            var result = new List<T>();
            using var workbook = new XLWorkbook(excelStream);
            var sheet = workbook.Worksheet(sheetKey);

            if (sheet == null)
                throw new Exception($"Sheet '{sheetKey}' not found.");

            var headerRow = sheet.Row(1);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var columnMap = new Dictionary<int, PropertyInfo>();

            for (int c = 1; c <= sheet.ColumnsUsed().Count(); c++)
            {
                var header = headerRow.Cell(c).GetString().Trim();

                if (columnTitles.TryGetValue(header, out var propName))
                {
                    var prop = props.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        columnMap[c] = prop;
                    }
                }
            }

            int row = 2;
            while (!sheet.Row(row).Cell(1).IsEmpty())
            {
                var item = new T();
                foreach (var kvp in columnMap)
                {
                    int colIndex = kvp.Key;
                    var prop = kvp.Value;
                    var cell = sheet.Row(row).Cell(colIndex);

                    try
                    {
                        var cellValue = cell.GetString();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            object? value;

                            if (targetType.IsEnum && translatedEnumValueMaps != null && translatedEnumValueMaps.TryGetValue(targetType, out var enumMap))
                            {
                                if (enumMap.TryGetValue(cellValue, out var enumValue))
                                {
                                    value = enumValue;
                                }
                                else
                                {
                                    _logger.LogWarning($"Không tìm thấy ánh xạ enum cho '{cellValue}' tại dòng {row}, cột {colIndex}");
                                    continue;
                                }
                            }
                            else
                            {
                                value = Convert.ChangeType(cellValue, targetType);
                            }

                            prop.SetValue(item, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Lỗi ánh xạ tại dòng {row}, cột {colIndex}");
                    }
                }

                result.Add(item);
                row++;
            }

            return await Task.FromResult(result);
        }


        public async Task<(Dictionary<string, ImportResult<object>>, byte[])> ImportExcelByTemplateAsync(
            Stream excelStream,
            Dictionary<string, (Type DtoType, Dictionary<string, string> ColumnTitles)> sheetConfigs,
            Dictionary<Type, Dictionary<string, object>>? translatedEnumValueMaps = null,
            bool validateData = true)
        {
            var results = new Dictionary<string, ImportResult<object>>();
            using var workbook = new XLWorkbook(excelStream);
            using var outputWorkbook = new XLWorkbook();

            foreach (var sheetConfig in sheetConfigs)
            {
                var sheetKey = sheetConfig.Key;
                var (dtoType, columnTitles) = sheetConfig.Value;
                var sheet = workbook.Worksheet(sheetKey);

                if (sheet == null)
                {
                    _logger.LogWarning($"Sheet '{sheetKey}' not found.");
                    continue;
                }

                var importResult = new ImportResult<object> { SheetName = sheetKey };
                var outputSheet = outputWorkbook.Worksheets.Add(sheetKey);
                var headerRow = sheet.Row(1);
                var props = dtoType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var columnMap = new Dictionary<int, PropertyInfo>();
                int colIndex = 1;
                for (int c = 1; c <= sheet.ColumnsUsed().Count(); c++)
                {
                    var header = headerRow.Cell(c).GetString().Trim();
                    outputSheet.Cell(1, colIndex).Value = header;
                    if (columnTitles.TryGetValue(header, out var propName))
                    {
                        var prop = props.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                        if (prop != null)
                        {
                            columnMap[c] = prop;
                        }
                    }
                    colIndex++;
                }
                outputSheet.Cell(1, colIndex).Value = I18NHelper.GetString(key: "Core_Import_Column_Result_Entry");
                outputSheet.Cell(1, colIndex + 1).Value = I18NHelper.GetString(key: "Core_Import_Column_Message_Entry");

                int row = 2;
                int outputRow = 2;
                while (!sheet.Row(row).Cell(1).IsEmpty())
                {
                    var item = Activator.CreateInstance(dtoType);
                    if (item == null)
                    {
                        throw new InvalidOperationException($"Failed to create instance of type {dtoType.Name} for row {row} in sheet '{sheetKey}'.");
                    }
                    var importRow = new ImportRow<object> { Data = item, Result = CImportResultType.Success };
                    var validationErrors = new List<string>();

                    colIndex = 1;
                    for (int c = 1; c <= sheet.ColumnsUsed().Count(); c++)
                    {
                        var cellValue = sheet.Row(row).Cell(c).GetString();
                        outputSheet.Cell(outputRow, colIndex).Value = cellValue;
                        colIndex++;
                    }

                    foreach (var kvp in columnMap)
                    {
                        int inputColIndex = kvp.Key;
                        var prop = kvp.Value;
                        var cell = sheet.Row(row).Cell(inputColIndex);

                        try
                        {
                            var cellValue = cell.GetString();
                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                object? value;

                                if (targetType.IsEnum && translatedEnumValueMaps != null &&
                                    translatedEnumValueMaps.TryGetValue(targetType, out var enumMap))
                                {
                                    if (enumMap.TryGetValue(cellValue, out var enumValue))
                                    {
                                        value = enumValue;
                                    }
                                    else
                                    {
                                        validationErrors.Add($"Invalid enum value '{cellValue}' for property {prop.Name} at row {row}, column {inputColIndex}.");
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (targetType == typeof(DateTimeOffset))
                                    {
                                        if (DateTimeOffset.TryParse(cellValue, out var dateValue))
                                        {
                                            value = dateValue;
                                        }
                                        else
                                        {
                                            validationErrors.Add($"Invalid DateTimeOffset format for property {prop.Name} at row {row}, column {inputColIndex}.");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        value = Convert.ChangeType(cellValue, targetType);
                                    }
                                }

                                prop.SetValue(item, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            validationErrors.Add($"Error mapping value for property {prop.Name} at row {row}, column {inputColIndex}: {ex.Message}");
                        }
                    }

                    if (validateData)
                    {
                        var validationContext = new ValidationContext(item);
                        var validationResults = new List<ValidationResult>();
                        if (!Validator.TryValidateObject(item, validationContext, validationResults, true))
                        {
                            validationErrors.AddRange(validationResults.Select(vr => vr.ErrorMessage ?? string.Empty));
                        }
                    }

                    if (validationErrors.Any())
                    {
                        importRow.Result = CImportResultType.Failed;
                        importRow.ErrorMessage = string.Join("; ", validationErrors);
                    }

                    // Write Result and ErrorMessage to output sheet with styling
                    var resultCell = outputSheet.Cell(outputRow, colIndex);
                    resultCell.Value = importRow.Result.ToDescription();
                    if (importRow.Result == CImportResultType.Success)
                    {
                        resultCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#C6EFCE");
                        resultCell.Style.Font.FontColor = XLColor.FromHtml("#006100");
                    }
                    else
                    {
                        resultCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC7CE");
                        resultCell.Style.Font.FontColor = XLColor.FromHtml("#9C0006");
                    }

                    if (importRow.Result == CImportResultType.Failed)
                    {
                        var errorCell = outputSheet.Cell(outputRow, colIndex + 1);
                        errorCell.Value = importRow.ErrorMessage;
                        errorCell.Style.Font.FontColor = XLColor.FromHtml("#9C0006");
                    }

                    importResult.Rows.Add(importRow);
                    row++;
                    outputRow++;
                }

                outputSheet.Columns().AdjustToContents();
                results.Add(sheetKey, importResult);
            }

            using var memoryStream = new MemoryStream();
            outputWorkbook.SaveAs(memoryStream);
            return await Task.FromResult((results, memoryStream.ToArray()));
        }

        private Stream GetTemplateStream(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Format: {Namespace}.{Folder}.{FileName}
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException($"Embedded resource '{fileName}' not found. Available: {string.Join(", ", assembly.GetManifestResourceNames())}");

            return assembly.GetManifestResourceStream(resourceName)!;
        }
    }
}