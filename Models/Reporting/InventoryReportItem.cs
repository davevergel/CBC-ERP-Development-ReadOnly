using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Reporting
{
    public class InventoryReportItem
    {
        public string ItemName { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
    }

}
