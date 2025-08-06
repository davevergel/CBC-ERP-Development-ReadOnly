using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories.HR
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly IDbConnection _db;

        public InterviewRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<Interview>> GetByCandidateIdAsync(int candidateId)
        {
            const string sql = "SELECT * FROM Interviews WHERE CandidateID = @CandidateID";
            return await _db.QueryAsync<Interview>(sql, new { CandidateID = candidateId });
        }

        public async Task AddAsync(Interview item)
        {
            const string sql = @"INSERT INTO Interviews (InterviewID) VALUES (@InterviewID)";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task UpdateAsync(Interview item)
        {
            const string sql = @"UPDATE Interviews SET InterviewID = @InterviewID WHERE InterviewID = @InterviewID";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Interviews WHERE InterviewID = @id";
            await _db.ExecuteAsync(sql, new { id });
        }
    }
}