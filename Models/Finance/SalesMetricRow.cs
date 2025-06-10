using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Finance
{
    public class SalesMetricRow
    {
        public DateTime Date { get; set; }
        public string Metric { get; set; }
        public decimal Amount { get; set; }
    }
}
