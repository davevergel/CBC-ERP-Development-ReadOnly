using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Finance
{
    public class DriposSalesMetric
    {
        public int Id { get; set; }
        public DateTime MetricDate { get; set; }
        public string MetricName { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        public int count { get; set; } // Assuming this is a count of entries for the metric
        public DateTime CreatedAt { get; set; }
    }
}
