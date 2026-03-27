using Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.RequestResponse;

public sealed record CreateSalesOrderRequest(
    int CustomerId,
    IEnumerable<SalesOrderItemRequest> Items,
    SalesOrderStatus SalesStatus,
    string? Description
    );
