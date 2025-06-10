using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class BatchSchedule
    {
        public int ScheduleID { get; set; }
        public int OrderID { get; set; } // Links to BigCommerce order.
        public int FinishedGoodID { get; set; } // Links to FinishedGoods
        public string FinishedGoodName { get; set; } // For display purposes
        public int Quantity { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
