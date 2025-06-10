using System.Collections.Generic;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Finance;

namespace CbcRoastersErp.Repositories.Finance
{
    public interface IJournalEntryRepository
    {
        Task<IEnumerable<JournalEntry>> GetAllAsync();
        Task<JournalEntry> GetByIdAsync(int id);
        Task AddAsync(JournalEntry entry, IEnumerable<JournalEntryLine> lines);
        Task DeleteAsync(int id);
    }
}
