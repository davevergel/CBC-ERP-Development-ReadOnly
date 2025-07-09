using System;
using System.Linq;
using Dapper;
using BCrypt.Net;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Services
{
    public class UserService
    {
        public static bool ValidateUser(string username, string password)
        {
            using (var db = DatabaseHelper.GetConnection())
            {
                string query = "SELECT Password FROM UserAccounts WHERE Username = @Username";
                var storedHash = db.Query<string>(query, new { Username = username }).FirstOrDefault();

                return storedHash != null && BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
        }

        public static void RegisterUser(string username, string password, int roleId)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            using (var db = DatabaseHelper.GetConnection())
            {
                string query = "INSERT INTO UserAccounts (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)";
                db.Execute(query, new { Username = username, Password = hashedPassword, RoleId = roleId });
            }
        }
    }
}
