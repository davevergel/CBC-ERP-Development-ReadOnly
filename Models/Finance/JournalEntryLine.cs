using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Finance
{
    public class JournalEntryLine
    {
        public int LineID { get; set; }
        public int JournalEntryID { get; set; }
        public int AccountID { get; set; }
        public decimal Amount { get; set; }
        public bool IsDebit { get; set; }
    }
}
