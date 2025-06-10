using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class Suppliers
    {
        public int Supplier_id { get; set; }
        public string Supplier_Name { get; set; }
        public string Contact_email { get; set; }
        public string Contact_phone { get; set; }
        public string Address { get; set; }
        public DateTime Created_at { get; set; }
    }
}
