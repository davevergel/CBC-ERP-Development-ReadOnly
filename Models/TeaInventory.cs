using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class TeaInventory
    {
        public int TeaID { get; set; }
        public string TeaName { get; set; }
        public string TeaType { get; set; }
        public string Origin { get; set; }
        public string Certifications { get; set; }
        public int Quantity { get; set; }
        public string BatchNumber { get; set; }
        public int StockLevel { get; set; }
        public int SupplierId { get; set; }
        public DateTime? DateReceived { get; set; }
        public string HarvestYear { get; set; }
        public decimal Price { get; set; }
    }
}
