using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions;
using Domain.Inventories;
using MediatR;

namespace Application.Inventories.DomainEventsHandlers;

public sealed class LowStockDomainEventHandler
    : INotificationHandler<LowStockDomainEvent>

{
    private readonly INotificationService _notificationService;

    public LowStockDomainEventHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(
        LowStockDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await _notificationService.NotifyLowStockAsync(
            notification.productId,
            notification.locationId,
            notification.currentStock);

    }
}
