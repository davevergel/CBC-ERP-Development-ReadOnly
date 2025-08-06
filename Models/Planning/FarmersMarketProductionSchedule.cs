using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Planning
{ 
    public class FarmersMarketProductionSchedule
    {
        public int Id { get; set; }
        public DateTime MarketDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Notes { get; set; }
    }
}
