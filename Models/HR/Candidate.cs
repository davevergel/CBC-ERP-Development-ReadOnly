using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.HR
{
    public class Candidate
    {
        public int CandidateID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ResumeLink { get; set; }
        public int AppliedJobID { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } // Applied, Interviewing, Hired, Rejected
    }
}
