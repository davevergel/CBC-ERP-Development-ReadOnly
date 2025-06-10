using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class BigCommerceOrder
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public DateTime? DateCreated { get; set; }
        public decimal TotalIncTax { get; set; }
    }

}
