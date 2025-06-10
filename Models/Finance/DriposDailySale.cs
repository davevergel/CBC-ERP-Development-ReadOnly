using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Finance
{
    public class DriposDailySale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public string Source { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
