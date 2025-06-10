using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models;

namespace CbcRoastersErp.Services
{
    public static class CurrentUserSession
    {
        public static UserModel? User { get; set; }

        public static bool HasPermission(string permissionName)
            => User?.Permissions.Contains(permissionName) ?? false;
    }
}
