using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Finance
{
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
  
        public bool IsActive { get; set; }
        // Navigation properties
        public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
    }
}
