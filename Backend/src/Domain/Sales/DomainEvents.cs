using Domain.Common.Events;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sales;

public sealed record SalesOrderCreatedDomainEvent(
    int SalesOrderId,
    int CustomerId,
    SalesOrderStatus status,
    DateTime OrderDate,
    decimal TotalAmount
    ) : IDomainEvent;
