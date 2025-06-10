using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Operations.Inventory
{
    public class InventoryKpiSummary
    {
        public double TotalGreenCoffee { get; set; }
        public double TotalTea { get; set; }
        public double TotalPackingStock { get; set; }
        public double TotalFinishedGoods { get; set; }
    }
}
