using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class BatchRoasting
    {
        public int BatchID { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? RoastDate { get; set; }
        public string Status { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int BatchSize { get; set; }
        public int ProfileID { get; set; } // linked to RoastingProfiles
        public string RoastLevel { get; set; } // e.g., Light, Medium, Dark
        public int FinishedGoodID { get; set; } // Links to FinishedGoods
        public int BigCommerceID { get; set; }  // BigCommerce Order Number
        public string ProfileName { get; set; } // For display purposes
        public string FinishedGoodName { get; set; } // For display purposes

    }

}
