namespace CbcRoastersErp.Services.Parsers
{
    public class InvoiceParserManager
    {
        private readonly List<IInvoiceParser> _parsers;

        public InvoiceParserManager()
        {
            _parsers = new List<IInvoiceParser>
        {
            new AdagioInvoiceParser(),
            new WebstaurantInvoiceParser()
            // Add future parsers here
        };
        }

        public InvoiceParseResult ParseInvoice(string filePath, string supplierName)
        {
            var parser = _parsers.FirstOrDefault(p => p.SupplierName.Contains(supplierName, StringComparison.OrdinalIgnoreCase));
            if (parser == null)
                throw new Exception($"No parser found for supplier: {supplierName}");

            return parser.Parse(filePath);
        }
    }
}
