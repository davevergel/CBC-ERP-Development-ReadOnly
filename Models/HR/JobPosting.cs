using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.HR
{
    public class JobPosting
    {
        public int JobID { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Open, Closed
        public DateTime PostedDate { get; set; }
    }
}
