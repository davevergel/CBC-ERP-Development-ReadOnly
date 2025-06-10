using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class WorkSchedules
    {
        public int ScheduleID { get; set; }
        public int EmployeeID { get; set; }
        public string TaskDescription { get; set; }
        public decimal TotalHours { get; set; }
        public int BreakDuration { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public string EmployeeFullName { get; set; } // For display purposes
    }
}
