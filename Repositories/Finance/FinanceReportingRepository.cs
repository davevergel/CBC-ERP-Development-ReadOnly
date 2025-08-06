using System.Data;
using Dapper;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories.Finance
{
    public class FinanceReportingRepository
    {
        private readonly IDbConnection _db;

        public FinanceReportingRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<decimal> GetTotalAsync(string accountType, DateTime startDate, DateTime endDate)
        {
            const string sql = @"
                SELECT SUM(CASE 
                            WHEN jl.IsDebit AND a.AccountType IN ('Asset', 'Expense') THEN jl.Amount
                            WHEN NOT jl.IsDebit AND a.AccountType IN ('Revenue', 'Liability', 'Equity') THEN jl.Amount
                            ELSE -jl.Amount
                           END) AS Total
                FROM JournalEntryLines jl
                INNER JOIN Accounts a ON jl.AccountID = a.AccountID
                INNER JOIN JournalEntries je ON jl.JournalEntryID = je.JournalEntryID
                WHERE a.AccountType = @accountType AND je.EntryDate BETWEEN @startDate AND @endDate";

            return await _db.ExecuteScalarAsync<decimal>(sql, new { accountType, startDate, endDate });
        }

        public async Task<IEnumerable<(string SupplierName, decimal Amount)>> GetOpenPOLiabilitiesAsync()
        {
            const string sql = @"
        SELECT s.Supplier_Name AS SupplierName,
               SUM(po.TotalAmount) AS Amount
        FROM purchase_orders po
        INNER JOIN Suppliers s ON po.Supplier_id = s.Supplier_id
        WHERE po.Status IN ('Pending', 'Approved')
        GROUP BY s.Supplier_Name";

            using var conn = DatabaseHelper.GetOpenConnection();
            return await conn.QueryAsync<(string SupplierName, decimal Amount)>(sql);
        }

        public async Task<decimal> GetTotalOpenPOLiabilitiesAsync()
        {
            const string sql = @"
        SELECT SUM(TotalAmount)
        FROM purchase_orders
        WHERE Status IN ('Pending', 'Approved');";

            using var conn = DatabaseHelper.GetOpenConnection();
            return await conn.ExecuteScalarAsync<decimal>(sql);
        }

    }
}
