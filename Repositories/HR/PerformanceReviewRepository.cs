using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using CbcRoastersErp.Models.HR;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories.HR
{
    public class PerformanceReviewRepository : IPerformanceReviewRepository
    {
        private readonly IDbConnection _db;

        public PerformanceReviewRepository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public async Task<IEnumerable<PerformanceReview>> GetByEmployeeIdAsync(int employeeId)
        {
            const string sql = "SELECT * FROM PerformanceReviews WHERE EmployeeID = @EmployeeID";
            return await _db.QueryAsync<PerformanceReview>(sql, new { EmployeeID = employeeId });
        }

        public async Task AddAsync(PerformanceReview review)
        {
            const string sql = @"INSERT INTO PerformanceReviews 
                (EmployeeID, ReviewDate, Reviewer, Score, Comments) 
                VALUES (@EmployeeID, @ReviewDate, @Reviewer, @Score, @Comments)";
            await _db.ExecuteAsync(sql, review);
        }

        public async Task UpdateAsync(PerformanceReview review)
        {
            const string sql = @"UPDATE PerformanceReviews 
                SET ReviewDate = @ReviewDate, Reviewer = @Reviewer, 
                    Score = @Score, Comments = @Comments 
                WHERE ReviewID = @ReviewID";
            await _db.ExecuteAsync(sql, review);
        }

        public async Task DeleteAsync(int reviewId)
        {
            const string sql = "DELETE FROM PerformanceReviews WHERE ReviewID = @ReviewID";
            await _db.ExecuteAsync(sql, new { ReviewID = reviewId });
        }
    }
}