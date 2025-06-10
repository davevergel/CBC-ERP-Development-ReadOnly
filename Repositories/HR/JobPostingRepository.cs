using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using CbcRoastersErp.Models.HR;
using CbcoastersErp.Repositories.HR;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories.HR
{
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly IDbConnection _db;

        public JobPostingRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<JobPosting>> GetAllAsync()
        {
            const string sql = "SELECT * FROM JobPostings";
            return await _db.QueryAsync<JobPosting>(sql);
        }

        public async Task<JobPosting> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM JobPostings WHERE JobID = @id";
            return await _db.QueryFirstOrDefaultAsync<JobPosting>(sql, new { id });
        }

        public async Task AddAsync(JobPosting item)
        {
            const string sql = @"INSERT INTO JobPostings (JobID) VALUES (@JobID)";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task UpdateAsync(JobPosting item)
        {
            const string sql = @"UPDATE JobPostings SET JobID = @JobID WHERE JobID = @JobID";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM JobPostings WHERE JobID = @id";
            await _db.ExecuteAsync(sql, new { id });
        }
    }
}