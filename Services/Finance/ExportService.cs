using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;

namespace CbcRoastersErp.Services.Finance
{
    public static class ExportService
    {
        public static void ExportToCsv<T>(IEnumerable<T> data, string folderPath, string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName + ".csv");

            using var writer = new StreamWriter(filePath);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            writer.WriteLine(string.Join(",", properties.Select(p => p.Name)));

            foreach (var item in data)
            {
                var values = properties.Select(p =>
                {
                    var value = p.GetValue(item);
                    return value is DateTime dt ? dt.ToString("yyyy-MM-dd") :
                           value is decimal d ? d.ToString(CultureInfo.InvariantCulture) :
                           value?.ToString()?.Replace(",", " ");
                });
                writer.WriteLine(string.Join(",", values));
            }
        }

        public static void ExportToExcel<T>(IEnumerable<T> data, string folderPath, string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName + ".xlsx");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Export");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            int col = 1;

            foreach (var prop in properties)
                worksheet.Cell(1, col++).Value = prop.Name;

            int row = 2;
            foreach (var item in data)
            {
                col = 1;
                foreach (var prop in properties)
                {
                    worksheet.Cell(row, col++).Value = (XLCellValue)prop.GetValue(item);
                }
                row++;
            }

            workbook.SaveAs(filePath);
        }
    }
}

