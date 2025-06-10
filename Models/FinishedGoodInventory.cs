using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class FinishedGoodInventory
    {
        public int InventoryID { get; set; }
        public int FinishedGoodID { get; set; } // Links to FinishedGoods
        public int BatchID { get; set; } // Links to BatchRoasting
        public int QuantityProduced { get; set; } // Quantity produced in this batch
        public DateTime? DateProduced { get; set; } // Date when the batch was produced
        public string BatchNumber { get; set; } // For display purposes
        public string ProductName { get; set; } // For display purposes
        public string RoastLevel { get; set; } // e.g., Light, Medium, Dark From 
    }
}
