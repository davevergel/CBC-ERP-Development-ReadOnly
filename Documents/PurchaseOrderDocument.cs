using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using CbcRoastersErp.Models.Purchasing;
using CbcRoastersErp.Models;
using System.IO;

public class PurchaseOrderDocument : IDocument
{
    private readonly PurchaseOrder _order;
    private readonly Suppliers _supplier;
    private readonly byte[] _logoBytes;

    public PurchaseOrderDocument(PurchaseOrder order, Suppliers supplier, byte[] logoBytes)
    {
        _order = order;
        _supplier = supplier;
        _logoBytes = logoBytes;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Size(PageSizes.A4);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header().Row(row =>
            {
                row.ConstantItem(80).Image(_logoBytes, ImageScaling.FitArea);
                row.RelativeItem().AlignCenter().Text($"Purchase Order #{_order.PurchaseOrderId}")
                    .FontSize(20).SemiBold().FontColor(Colors.Brown.Medium);
            });

            page.Content().Column(col =>
            {
                col.Spacing(15);

                col.Item().Row(row =>
                {
                    row.RelativeColumn().Column(inner =>
                    {
                        inner.Item().Text($"Supplier: {_supplier?.Supplier_Name}");
                        inner.Item().Text($"Date: {_order.OrderDate:yyyy-MM-dd}");
                        inner.Item().Text($"Status: {_order.Status}");
                    });
                });

                col.Item().LineHorizontal(1);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Product").SemiBold();
                        header.Cell().Text("Qty").SemiBold();
                        header.Cell().Text("Unit Price").SemiBold();
                        header.Cell().Text("Total").SemiBold();
                    });

                    foreach (var item in _order.Items)
                    {
                        table.Cell().Text(item.ProductName);
                        table.Cell().Text(item.Quantity.ToString());
                        table.Cell().Text($"{item.UnitPrice:C}");
                        table.Cell().Text($"{(item.Quantity * item.UnitPrice):C}");
                    }
                });

                col.Item().AlignRight().Column(inner =>
                {
                    inner.Item().Text($"Subtotal: {_order.Subtotal:C}");
                    inner.Item().Text($"Tax: {_order.TaxAmount:C}");
                    inner.Item().Text($"Shipping: {_order.ShippingCost:C}");
                    inner.Item().Text($"Total: {_order.TotalAmount:C}").Bold();
                });
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("CBC Roasters ERP").SemiBold();
                x.Span("  —  Printed ").FontSize(10);
                x.Span($"{System.DateTime.Now:g}").FontSize(10);
            });
        });
    }
}




