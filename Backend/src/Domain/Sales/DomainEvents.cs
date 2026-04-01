using Domain.Shared.Events;

namespace Domain.Sales;

public sealed record SalesOrderCreatedDomainEvent(
    int SalesOrderId,
    int? CustomerId,
    SalesOrderStatus Status,
    DateTime OrderDate,
    decimal TotalAmount) : DomainEvent;

public sealed record SalesOrderConfirmedDomainEvent(
    int SalesOrderId) : DomainEvent;

public sealed record SalesOrderShippedDomainEvent(
    int SalesOrderId,
    string? TrackingNumber) : DomainEvent;

public sealed record SalesOrderCancelledDomainEvent(
    int SalesOrderId,
    SalesOrderStatus PreviousStatus) : DomainEvent;

public sealed record SalesOrderCompletedDomainEvent(
    int SalesOrderId) : DomainEvent;

public sealed record SalesOrderReturnedDomainEvent(
    int SalesOrderId) : DomainEvent;
