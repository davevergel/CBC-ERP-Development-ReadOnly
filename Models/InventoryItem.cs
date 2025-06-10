using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class InventoryItem
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; } // Green Coffee, Tea, Packing, Finished Goods
        public int SupplierID { get; set; }
        public int QuantityInStock { get; set; }
        public decimal CostPerUnit { get; set; }
        public string StorageLocation { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
