using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Helpers;

namespace CBCRoastersErp.Repositories.HR
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly IDbConnection _db;

        public CandidateRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<Candidate>> GetByJobIdAsync(int jobId)
        {
            const string sql = "SELECT * FROM Candidates WHERE JobID = @JobID";
            return await _db.QueryAsync<Candidate>(sql, new { JobID = jobId });
        }

        public async Task<Candidate> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Candidates WHERE CandidateID = @id";
            return await _db.QueryFirstOrDefaultAsync<Candidate>(sql, new { id });
        }
        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Candidates";
            return await _db.QueryAsync<Candidate>(sql);
        }
        public async Task<IEnumerable<Candidate>> GetByPageAsync(int pageNumber, int pageSize)
        {
            try
            {                
                const string sql = @"
                            SELECT           
                                cd.CandidateID, cd.FirstName, cd.LastName, cd.Email, cd.Phone,
                                cd.ResumeLink, cd.AppliedJobID, cd.ApplicationDate, cd.Status,
                                job.JobID, job.Title as JobTitle, job.Description as JobDescription
                                FROM Candidates cd
                                LEFT JOIN JobPostings job ON cd.AppliedJobID = job.JobID
                                ORDER BY CandidateID LIMIT @Offset, @PageSize";
                return await _db.QueryAsync<Candidate>(sql, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<Candidate>();
            }
        }
        public async Task<int> GetTotalCountAsync()
        {
            const string sql = "SELECT COUNT(*) FROM Candidates";
            return await _db.ExecuteScalarAsync<int>(sql);
        }

        public async Task AddAsync(Candidate item)
        {
            const string sql = @"INSERT INTO Candidates (CandidateID) VALUES (@CandidateID)";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task UpdateAsync(Candidate item)
        {
            const string sql = @"UPDATE Candidates SET CandidateID = @CandidateID WHERE CandidateID = @CandidateID";
            await _db.ExecuteAsync(sql, item);
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Candidates WHERE CandidateID = @id";
            await _db.ExecuteAsync(sql, new { id });
        }

        public async Task<IEnumerable<JobPosting>> GetAllJobPostingsAsync()
        {
            try
            {
                const string sql = "SELECT * FROM JobPostings";
                return await _db.QueryAsync<JobPosting>(sql);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, "System", "Error");
                return Enumerable.Empty<JobPosting>();
            }
        }
    }
}