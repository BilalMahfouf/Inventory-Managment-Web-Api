using Application.PagedLists;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.Queries;

public interface ISalesOrderQueries
{
    Task<Result<object>> GetDahsboardSummaryAsync(
        CancellationToken cancellationToken = default);
    Task<Result<PagedList<object>>> GetOrdersTableAsync(
        TableRequest request,
        CancellationToken cancellationToken = default);
}
