using System.Data;
using Dapper;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Repositories.HR;

namespace CBCRoastersERP.Repositories.HR
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _db;

        public EmployeeRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
            const string sql = "SELECT * FROM Employees";
            return await _db.QueryAsync<Employee>(sql);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                ApplicationLogger.Log(ex, "Error fetching all employees");
                return Enumerable.Empty<Employee>();
            }
            
        }
        public async Task<IEnumerable<Employee>> GetByPageAsync(int pageNumber, int pageSize)
        {
            const string sql = "SELECT * FROM Employees ORDER BY EmployeeID LIMIT @Offset, @PageSize";
            return await _db.QueryAsync<Employee>(sql, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize });
        }
        public async Task<int> GetTotalCountAsync()
        {
            const string sql = "SELECT COUNT(*) FROM Employees";
            return await _db.ExecuteScalarAsync<int>(sql);
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Employees WHERE EmployeeID = @id";
            return await _db.QueryFirstOrDefaultAsync<Employee>(sql, new { id });
        }

        public async Task AddAsync(Employee item)
        {
            const string sql = @"INSERT INTO Employees (EmployeeID) VALUES (@EmployeeID)";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task UpdateAsync(Employee item)
        {
            const string sql = @"UPDATE Employees SET EmployeeID = @EmployeeID WHERE EmployeeID = @EmployeeID";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Employees WHERE EmployeeID = @id";
            await _db.ExecuteAsync(sql, new { id });
        }
    }
}