using CbcRoastersErp.Models.HR;

namespace CbcoastersErp.Repositories.HR
{
    public interface IJobPostingRepository
    {
        Task<IEnumerable<JobPosting>> GetAllAsync();
        Task<JobPosting> GetByIdAsync(int id);
        Task AddAsync(JobPosting posting);
        Task UpdateAsync(JobPosting posting);
        Task DeleteAsync(int id);
    }
}