using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class BigCommerceOrders
    {
        public int OrderID { get; set; }
        public int BigCommerceID { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } // display name for the customer
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
    }
}
