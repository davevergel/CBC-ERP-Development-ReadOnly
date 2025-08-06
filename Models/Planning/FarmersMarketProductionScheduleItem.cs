using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Planning
{
    public class FarmersMarketProductionScheduleItem
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int ProductId { get; set; }
        public string UsageType { get; set; }
        public decimal PlannedQuantity { get; set; }
        public decimal? ActualProducedQuantity { get; set; }
        public DateTime? RoastDate { get; set; }
        public string Notes { get; set; }

        // Optional: Navigation property for ViewModel convenience
        public string ProductName { get; set; }
    }
}
