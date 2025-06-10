using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace CbcRoastersErp.Repositories
{
    public class RolesRepository
    {
        private readonly IDbConnection _dbConnection;

        public RolesRepository()
        {
            _dbConnection = DatabaseHelper.GetConnection();
        }

        public IEnumerable<Role> GetAllRoles()
        {
            const string query = "SELECT * FROM Roles";
            return _dbConnection.Query<Role>(query);
        }

        public void AddRole(Role role)
        {
            const string query = "INSERT INTO Roles (RoleName, PermissionLevel, Description) VALUES (@RoleName, @PermissionLevel, @Description)";
            _dbConnection.Execute(query, role);
        }

        public void UpdateRole(Role role)
        {
            const string query = "UPDATE Roles SET RoleName = @RoleName, PermissionLevel = @PermissionLevel, Description = @Description WHERE RoleID = @RoleID";
            _dbConnection.Execute(query, role);
        }

        public void DeleteRole(int roleId)
        {
            const string query = "DELETE FROM Roles WHERE RoleID = @RoleID";
            _dbConnection.Execute(query, new { RoleID = roleId });
        }
    }
}

