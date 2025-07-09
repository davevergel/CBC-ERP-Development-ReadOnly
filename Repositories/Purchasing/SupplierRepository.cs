using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using Dapper;

namespace CbcRoastersErp.Repositories.Purchasing
{
    public class SupplierRepository
    {
        public async Task<List<Suppliers>> GetAllAsync()
        {
            try
            {
                using var connection = DatabaseHelper.GetConnection();
                string sql = "SELECT * FROM Suppliers ORDER BY Supplier_Name";
                var suppliers = await connection.QueryAsync<Suppliers>(sql);
                return suppliers.ToList();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "GetAllAsync", nameof(SupplierRepository), Environment.UserName);
                return new List<Suppliers>();
            }
        }
    }
}
