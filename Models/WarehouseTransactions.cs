using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class WarehouseTransaction
    {
        public int TransactionID { get; set; }
        public string TransactionType { get; set; } // 'IN' or 'OUT'
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
