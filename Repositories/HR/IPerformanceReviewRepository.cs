using CbcRoastersErp.Models.HR;

namespace CbcRoastersErp.Repositories.HR
{
    public interface IPerformanceReviewRepository
    {
        Task<IEnumerable<PerformanceReview>> GetByEmployeeIdAsync(int employeeId);
        Task AddAsync(PerformanceReview review);
        Task UpdateAsync(PerformanceReview review);
        Task DeleteAsync(int reviewId);
    }
}