using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dashboard.Contracts
{
    public interface IDashboardQueries
    {
        Task<object> GetDashboardSummaryAsync(
            CancellationToken cancellationToken = default);
        Task<Result<object>> GetTodayPerformanceAsync(
            CancellationToken cancellationToken = default);
                



    }
}
