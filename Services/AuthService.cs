using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models;

namespace CbcRoastersErp.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool VerifyUser(string username, string password)
        {
            var user = _context.UserAccounts.SingleOrDefault(u => u.Username == username);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
