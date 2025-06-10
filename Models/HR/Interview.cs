using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.HR
{
    public class Interview
    {
        public int InterviewID { get; set; }
        public int CandidateID { get; set; }
        public string CandidateName { get; set; } // Full name of the candidate 
        public int JobID { get; set; }
        public string InterviewType { get; set; } // Phone, In-Person, Video

        public DateTime InterviewDate { get; set; }
        public string Interviewer { get; set; }
        public string Notes { get; set; }
        public string Outcome { get; set; } // Pending, Passed, Failed
    }
}
