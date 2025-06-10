using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Finance;

namespace CbcRoastersErp.Repositories.Finance
{
    public interface IDriposSalesMetricsRepository
    {
        Task<IEnumerable<DriposSalesMetric>> GetAllAsync();
        Task InsertAsync(DriposSalesMetric metric);
        Task InsertAsync(IEnumerable<DriposSalesMetric> metrics);
        Task<IEnumerable<DriposSalesMetric>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}
