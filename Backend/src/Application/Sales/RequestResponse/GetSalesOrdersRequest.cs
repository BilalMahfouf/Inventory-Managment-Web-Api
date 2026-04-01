namespace Application.Sales.RequestResponse;

public sealed record GetSalesOrdersRequest(
    SalesOrderStatus? Status,
    int? CustomerId,
    DateTime? DateFrom,
    DateTime? DateTo,
    int? PageNumber,
    int? PageSize,
    string? SortColumn,
    string? SortOrder);