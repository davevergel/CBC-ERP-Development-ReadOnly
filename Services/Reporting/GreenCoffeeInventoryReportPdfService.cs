using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using CbcRoastersErp.Reports;


namespace CbcRoastersErp.Services.Reporting
{
    public static class GreenCoffeeInventoryReportPdfService
    {
        public static string Generate(List<GreenCoffeeReportItem> data)
        {
            var outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "Pdf");
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string filePath = Path.Combine(outputDir, "GreenCoffeeInventory.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Green Coffee Inventory Report")
                                .FontSize(20)
                                .Bold()
                                .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Header row
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Lot Code").Bold();
                            header.Cell().Element(CellStyle).Text("Origin").Bold();
                            header.Cell().Element(CellStyle).Text("Quantity (lbs)").Bold();
                            header.Cell().Element(CellStyle).Text("Stock Level").Bold();
                            header.Cell().Element(CellStyle).Text("Date Received").Bold();
                        });

                        // Data rows
                        foreach (var item in data)
                        {
                            table.Cell().Element(CellStyle).Text($"(item.CoffeeName)");
                            table.Cell().Element(CellStyle).Text(item.BatchNumber ?? "");
                            table.Cell().Element(CellStyle).Text(item.Origin ?? "");
                            table.Cell().Element(CellStyle).Text($"{item.Quantity:n0}");
                            table.Cell().Element(CellStyle).Text($"{item.StockLevel}");
                            table.Cell().Element(CellStyle).Text($"{item.SupplierName}");
                            table.Cell().Element(CellStyle).Text(item.DateReceived?.ToString("yyyy-MM-dd") ?? "");
                        }

                        static IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                    });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }

}
