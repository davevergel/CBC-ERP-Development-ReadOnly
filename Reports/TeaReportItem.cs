using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Reports
{
    public class TeaReportItem : IInventoryReportItem
    {
        public string TeaName { get; set; }
        public string TeaType { get; set; }
        public string Origin { get; set; }
        public string Certifications { get; set; }
        public int StockLevel { get; set; }
        public DateTime? DateReceived { get; set; }
        public decimal Price { get; set; }
    }
}
