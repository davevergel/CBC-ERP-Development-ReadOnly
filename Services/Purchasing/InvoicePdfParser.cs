using System.Collections.ObjectModel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using CbcRoastersErp.Models.Purchasing;
using CbcRoastersErp.Models;

namespace CbcRoastersErp.Services
{
    public class InvoicePdfParserService
    {
        public InvoiceParseResult ParseInvoice(string filePath, List<Suppliers> suppliers)
        {
            var result = new InvoiceParseResult();
            var items = new ObservableCollection<PurchaseOrderItem>();

            using var pdfReader = new PdfReader(filePath);
            using var pdfDoc = new PdfDocument(pdfReader);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var strategy = new LocationTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);

                // Dynamic supplier detection
                foreach (var supplier in suppliers)
                {
                    if (!string.IsNullOrWhiteSpace(supplier.Supplier_Name) &&
                        text.Contains(supplier.Supplier_Name, StringComparison.OrdinalIgnoreCase))
                    {
                        result.SupplierName = supplier.Supplier_Name;
                        break;
                    }
                }

                var lines = text.Split('\n');

                foreach (var line in lines)
                {
                    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    // WebstaurantStore-specific item parsing
                    if (tokens.Length >= 6 &&
                        decimal.TryParse(tokens[^3].Replace("$", ""), out var unitPrice) &&
                        int.TryParse(tokens[^2], out var qty) &&
                        decimal.TryParse(tokens[^1].Replace("$", ""), out var total))
                    {
                        var productName = string.Join(" ", tokens, 1, tokens.Length - 5);

                        items.Add(new PurchaseOrderItem
                        {
                            ProductName = productName,
                            Quantity = qty,
                            UnitPrice = unitPrice
                        });
                    }

                    // Totals parsing
                    if (line.ToLower().Contains("subtotal"))
                        result.Subtotal = ExtractLastDecimal(tokens);
                    else if (line.ToLower().Contains("estimated tax"))
                        result.TaxAmount = ExtractLastDecimal(tokens);
                    else if (line.ToLower().Contains("shipping"))
                        result.ShippingCost = ExtractLastDecimal(tokens);
                    else if (line.ToLower().Contains("total"))
                        result.TotalAmount = ExtractLastDecimal(tokens);
                }
            }

            result.Items = items;
            return result;
        }


        private decimal ExtractLastDecimal(string[] tokens)
        {
            return decimal.TryParse(tokens[^1], out var value) ? value : 0m;
        }
    }

    public class InvoiceParseResult
    {
        public string SupplierName { get; set; } // NEW
        public ObservableCollection<PurchaseOrderItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
