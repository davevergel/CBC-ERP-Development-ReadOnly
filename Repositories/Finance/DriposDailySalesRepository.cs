using Dapper;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories.Finance
{
    public class DriposDailySalesRepository : IDriposDailySalesRepository
    {
        public async Task<IEnumerable<DriposDailySale>> GetAllAsync()
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                return await conn.QueryAsync<DriposDailySale>("SELECT * FROM dripos_daily_sales ORDER BY SaleDate");
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, "System");
                return new List<DriposDailySale>();
            }
        }

        public async Task InsertAsync(DriposDailySale sale)
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                const string sql = "INSERT INTO dripos_daily_sales (SaleDate, Amount, Source) VALUES (@SaleDate, @Amount, @Source)";
                await conn.ExecuteAsync(sql, sale);
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, "System");
            }
        }
    }
}
