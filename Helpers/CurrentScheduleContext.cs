using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Helpers
{
    public sealed class CurrentScheduleContext
    {
        private static readonly Lazy<CurrentScheduleContext> _instance = new(() => new CurrentScheduleContext());
        public static CurrentScheduleContext Instance => _instance.Value;

        public int SelectedScheduleId { get; set; }
    }
}
