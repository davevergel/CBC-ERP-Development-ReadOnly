using CbcRoastersErp.Models.Purchasing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Collections.ObjectModel;

namespace CbcRoastersErp.Services.Parsers
{
    public class AdagioInvoiceParser : IInvoiceParser
    {
        public string SupplierName => "Adagio Teas";

        public InvoiceParseResult Parse(string filePath)
        {
            var result = new InvoiceParseResult();
            var items = new ObservableCollection<PurchaseOrderItem>();

            using var pdfReader = new PdfReader(filePath);
            using var pdfDoc = new PdfDocument(pdfReader);

            // Example parsing logic for Adagio
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var strategy = new LocationTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);

                var lines = text.Split('\n');

                foreach (var line in lines)
                {
                    // TODO: Implement supplier-specific parsing logic here
                }
            }

            result.Items = items;
            return result;
        }
    }
}
