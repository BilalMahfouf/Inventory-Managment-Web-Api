using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries
{
    public interface IProductQueries
    {
        Task<Result<object>> GetProductDashboardSummaryAsync(
            CancellationToken cancellationToken = default);
    }
}
