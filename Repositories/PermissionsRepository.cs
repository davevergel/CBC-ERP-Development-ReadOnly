using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using Dapper;

namespace CbcRoastersErp.Repositories
{
    public class PermissionsRepository
    {
        public List<Permission> GetAllPermissions()
        {
            using var db = DatabaseHelper.GetOpenConnection();
            return db.Query<Permission>("SELECT * FROM Permissions").ToList();
        }

        public List<Permission> GetPermissionsForRole(int roleId)
        {
            using var db = DatabaseHelper.GetOpenConnection();
            return db.Query<Permission>(
                @"SELECT p.* FROM Permissions p
          JOIN RolePermissions rp ON p.PermissionId = rp.PermissionId
          WHERE rp.RoleId = @RoleId", new { RoleId = roleId }).ToList();
        }

        public void UpdateRolePermissions(int roleId, List<int> permissionIds)
        {
            using var db = DatabaseHelper.GetOpenConnection();
            using var trans = db.BeginTransaction();

            db.Execute("DELETE FROM RolePermissions WHERE RoleId = @RoleId", new { RoleId = roleId }, trans);

            foreach (var pid in permissionIds)
            {
                db.Execute("INSERT INTO RolePermissions (RoleId, PermissionId) VALUES (@RoleId, @PermissionId)",
                    new { RoleId = roleId, PermissionId = pid }, trans);
            }

            trans.Commit();
        }

    }
}
