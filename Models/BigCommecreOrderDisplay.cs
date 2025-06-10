using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class BigCommerceOrderDisplay
    {
        public int OrderID { get; set; }
        public int BigCommerceID { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
    }
}
