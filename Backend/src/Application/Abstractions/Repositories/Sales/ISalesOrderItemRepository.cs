using Application.Abstractions.Repositories.Base;
using Application.Results;
using Domain.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories.Sales
{
    public interface ISalesOrderItemRepository : IBaseRepository<SalesOrderItem>
    {
        Task<IEnumerable<object>> GetTopSellingProductsAsync(int? numberOfProducts
            , CancellationToken cancellationToken = default);

        Task<decimal> GetTotalRevenuesAsync(
            CancellationToken cancellationToken = default);
        
    }
}
