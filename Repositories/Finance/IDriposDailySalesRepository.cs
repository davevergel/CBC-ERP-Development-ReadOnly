﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CbcRoastersErp.Models.Finance;

namespace CbcRoastersErp.Repositories.Finance
{
    public interface IDriposDailySalesRepository
    {
        Task<IEnumerable<DriposDailySale>> GetAllAsync();
        Task InsertAsync(DriposDailySale sale);
    }
}
