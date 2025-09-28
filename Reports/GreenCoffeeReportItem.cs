using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Reports
{
    public class GreenCoffeeReportItem : IInventoryReportItem
    {
        public string CoffeeName { get; set; }
        public string Origin { get; set; }
        public string Variety { get; set; }
        public string ProcessType { get; set; }
        public string BatchNumber { get; set; }
        public int Quantity { get; set; }
        public int StockLevel { get; set; }
        public string SupplierName { get; set; }
        public DateTime? DateReceived { get; set; }
        public decimal Price { get; set; }
    }
}
