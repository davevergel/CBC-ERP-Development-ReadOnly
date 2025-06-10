using CbcRoastersErp.Models.HR;

namespace CbcRoastersErp.Repositories.HR
{
    public interface IInterviewRepository
    {
        Task<IEnumerable<Interview>> GetByCandidateIdAsync(int candidateId);
        Task AddAsync(Interview interview);
        Task UpdateAsync(Interview interview);
        Task DeleteAsync(int interviewId);
    }
}