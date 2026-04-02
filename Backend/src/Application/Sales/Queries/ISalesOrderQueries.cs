using Application.Shared.Paging;
using Domain.Shared.Results;
using Application.Sales.RequestResponse;

namespace Application.Sales.Queries;

public interface ISalesOrderQueries
{
    Task<Result<PagedList<SalesOrderTableResponse>>> GetSalesOrdersAsync(
        TableRequest request,
        SalesOrderStatus? status,
        int? customerId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken = default);
    Task<Result<SalesOrderReadResponse>> GetSalesOrderByIdAsync(
        int orderId,
        CancellationToken cancellationToken = default);
    Task<Result<object>> GetDahsboardSummaryAsync(
               CancellationToken cancellationToken = default);

}
