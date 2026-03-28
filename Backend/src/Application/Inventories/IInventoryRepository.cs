using Application.Shared.Contracts;
using Domain.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Inventories
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<decimal> GetInventoryValuationAsync
            (CancellationToken cancellationToken = default);
        Task<decimal> GetInventoryCostAsync
            (CancellationToken cancellationToken = default);
    }
}
