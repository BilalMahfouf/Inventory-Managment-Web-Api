using Application.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales;

public interface ISalesOrderQueries
{
    Task<Result<object>> GetDashboardSumamryAsync(
        CancellationToken cancellationToken = default);
    Task<Result<object>> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
