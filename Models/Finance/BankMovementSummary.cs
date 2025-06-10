using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models.Finance
{
    public class BankMovementSummary
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public decimal NetMovement { get; set; }
    }
}
