using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class FinishedGoods
    {
        public int FinishedGoodID { get; set; }
        public int BigCommProdID { get; set; }
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
        public int BatchID { get; set; }
        public int StockLevel { get; set; }
        public int ProfileID { get; set; } // Now linked to RoastingProfiles
        public string? SKU { get; set; }
        public string BatchNumber { get; set; } // For display purposes
        public DateTime? ProductionDate { get; set; } // Nullable to handle unproduced items
        public string ProfileName { get; set; } // For display purposes
        public string RoastLevel { get; set; } // e.g., Light, Medium, Dark
    }

}
