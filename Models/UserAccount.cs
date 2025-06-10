using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CbcRoastersErp.Models
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } // Optional: to store role name for easier access
        public string IsActive { get; set; } = "Yes"; // Default to active
        public DateTime? DateLastLogin { get; set; } // Nullable to allow for no login date
        public DateTime? DateCreated { get; set; }
    }
}
