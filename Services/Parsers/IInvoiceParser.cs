namespace CbcRoastersErp.Services.Parsers
{
    public interface IInvoiceParser
    {
        string SupplierName { get; }
        InvoiceParseResult Parse(string filePath);
    }
}
