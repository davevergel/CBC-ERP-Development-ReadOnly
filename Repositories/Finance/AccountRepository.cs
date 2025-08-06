using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Helpers;
using Dapper;
using System.Security.Principal;

namespace CbcRoastersErp.Repositories.Finance
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _db;

        public AccountRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<Account>> GetAllAsync() =>
            await _db.QueryAsync<Account>("SELECT * FROM Accounts WHERE IsActive = 1");

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _db.QueryFirstOrDefaultAsync<Account>("SELECT * FROM Accounts WHERE AccountID = @id", new { id });
        }

        public async Task AddAsync(Account account)
        {
            try {
            const string sql = @"INSERT INTO Accounts (AccountName, AccountType, IsActive)
                                 VALUES (@AccountName, @AccountType, @IsActive)";
            await _db.ExecuteAsync(sql, account);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ApplicationLogger.Log(ex, nameof(AddAsync), nameof(AccountRepository), Environment.UserName);
            }
        }

        public async Task UpdateAsync(Account account)
        {
            try
            {
                const string sql = @"UPDATE Accounts SET AccountName = @AccountName, 
                                 AccountType = @AccountType, IsActive = @IsActive
                                 WHERE AccountID = @AccountID";
                await _db.ExecuteAsync(sql, account);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ApplicationLogger.Log(ex, nameof(UpdateAsync), nameof(AccountRepository), Environment.UserName);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                const string sql = "UPDATE Accounts SET IsActive = FALSE WHERE AccountID = @id";
                await _db.ExecuteAsync(sql, new { id });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                ApplicationLogger.Log(ex, nameof(DeleteAsync), nameof(AccountRepository), Environment.UserName);
            }
        }
    }
}
