using System.Reflection;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

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