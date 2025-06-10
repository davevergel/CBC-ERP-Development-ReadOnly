using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Finance;

namespace CbcRoastersErp.Repositories.Finance
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account> GetByIdAsync(int id);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);
    }
}
