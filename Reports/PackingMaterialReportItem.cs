using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Reports
{
    public class PackingMaterialReportItem : IInventoryReportItem
    {
        public string MaterialName { get; set; }
        public int StockLevel { get; set; }
        public string Unit { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
