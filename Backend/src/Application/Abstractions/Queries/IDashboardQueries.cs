using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries
{
    public interface IDashboardQueries
    {
        Task<object> GetDashboardSummaryAsync(
            CancellationToken cancellationToken = default);
        Task<Result<object>> GetTodayPerformanceAsync(
            CancellationToken cancellationToken = default);
                



    }
}
