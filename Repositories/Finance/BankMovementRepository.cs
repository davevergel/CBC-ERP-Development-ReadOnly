using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.Finance;
using Dapper;

namespace CbcRoastersErp.Repositories.Finance
{
    public class BankMovementRepository : IBankMovementRepository
    {
        public async Task<IEnumerable<BankMovementSummary>> GetNetMovementsAsync()
        {
            try
            {
                using var conn = DatabaseHelper.GetOpenConnection();
                return await conn.QueryAsync<BankMovementSummary>("SELECT * FROM bank_movements_summary");
            }
            catch (System.Exception ex)
            {
                ApplicationLogger.Log(ex, "System");
                return new List<BankMovementSummary>();
            }
        }
    }
}

