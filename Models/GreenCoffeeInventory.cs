using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class GreenCoffeeInventory
    {
        public int GreenCoffeeID { get; set; }
        public string CoffeeName { get; set; }
        public string Origin { get; set; }
        public string Variety { get; set; }
        public string ProcessType { get; set; }
        public string Certifications { get; set; }
        public int Quantity { get; set; }
        public string HarvestYear { get; set; }
        public string BatchNumber { get; set; }
        public int StockLevel { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } // For display purposes
        public DateTime? DateReceived { get; set; }
        public decimal Price { get; set; }
    }
}
