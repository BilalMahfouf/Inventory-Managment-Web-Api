using Application.PagedLists;
using Application.Results;
using Application.Sales.RequestResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.Queries;

public interface ISalesOrderQueries
{
    Task<Result<object>> GetDahsboardSummaryAsync(
        CancellationToken cancellationToken = default);
    Task<Result<PagedList<SalesOrderTableResponse>>> GetOrdersTableAsync(
        TableRequest request,
        CancellationToken cancellationToken = default);
}
