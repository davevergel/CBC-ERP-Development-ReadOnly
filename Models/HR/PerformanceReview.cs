using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.HR
{
    public class PerformanceReview
    {
        public int ReviewID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Reviewer { get; set; }
        public int Score { get; set; } // 1–10 scale
        public string Comments { get; set; }
    }
}
