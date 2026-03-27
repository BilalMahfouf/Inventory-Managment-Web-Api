using Domain.Shared.Events;
using Domain.Shared.Enums;
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
    ) : DomainEvent;

public sealed record SalesOrderCancelledDomainEvent(
    int SalesOrderId,
    SalesOrderStatus prevStatus
    ) : DomainEvent;

public sealed record SalesOrderCompletedDomainEvent(
    int SalesOrderId
    ) : DomainEvent;
