using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Reports
{
    public class FinishedGoodReportItem : IInventoryReportItem
    {
        public string BatchNumber { get; set; }
        public string ProductName { get; set; }
        public string RoastLevel { get; set; }
        public int QuantityProduced { get; set; }
        public DateTime? DateProduced { get; set; }
    }
}
