using System.Collections.Generic;
using System.Threading.Tasks;
using CbcRoastersErp.Models.HR;


namespace CBCRoastersErp.Repositories.HR
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<Candidate>> GetByJobIdAsync(int jobId);
        Task AddAsync(Candidate candidate);
        Task UpdateAsync(Candidate candidate);
        Task DeleteAsync(int id);
    }
}