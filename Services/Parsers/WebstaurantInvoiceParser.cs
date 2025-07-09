using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Purchasing;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;

namespace CbcRoastersErp.Services.Parsers
{
    public class WebstaurantInvoiceParser : IInvoiceParser
    {
        public string SupplierName => "WebstaurantStore";

        public InvoiceParseResult Parse(string filePath)
        {
            var result = new InvoiceParseResult();
            var items = new ObservableCollection<PurchaseOrderItem>();

            using var pdfReader = new PdfReader(filePath);
            using var pdfDoc = new PdfDocument(pdfReader);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var strategy = new LocationTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);

                var lines = text.Split('\n');

                foreach (var line in lines)
                {
                    var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length < 6 || tokens[0] == "Item" || tokens[1] == "Number")
                        continue;

                    if (decimal.TryParse(tokens[^4].Replace("$", ""), out var unitPrice) &&
                        int.TryParse(tokens[^3], out var qty) &&
                        decimal.TryParse(tokens[^1].Replace("$", ""), out var total))
                    {
                        var descriptionTokens = tokens.Skip(1).Take(tokens.Length - 5);
                        var productName = string.Join(" ", descriptionTokens);

                        items.Add(new PurchaseOrderItem
                        {
                            ProductName = productName,
                            Quantity = qty,
                            UnitPrice = unitPrice
                        });
                    }

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
            return decimal.TryParse(tokens[^1].Replace("$", ""), out var value) ? value : 0m;
        }
    }

}
