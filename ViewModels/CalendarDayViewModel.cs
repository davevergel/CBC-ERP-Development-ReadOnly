using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models;

namespace CbcRoastersErp.ViewModels
{
    public class CalendarDayViewModel
    {
        public DateTime Date { get; set; }
        public List<WorkSchedules> Schedules { get; set; } = new();
    }
}
