using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions;
using Application.Abstractions.Queries;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Notifications;
using Domain.Inventories;
using MediatR;

namespace Application.Inventories.DomainEventsHandlers;

public sealed class LowStockDomainEventHandler
    : INotificationHandler<LowStockDomainEvent>

{
    private readonly INotificationService _notificationService;
    private readonly IInventoryQueries _query;


    public LowStockDomainEventHandler(
        INotificationService notificationService,
        IInventoryQueries query)
    {
        _notificationService = notificationService;
        _query = query;
    }

    public async Task Handle(
        LowStockDomainEvent notification,
        CancellationToken cancellationToken)
    {
        if (notification is null)
        {
            return;
        }
        var inventory = await _query.GetLowStockMessageDetailsAsync(
            notification.productId,
            notification.locationId,
            cancellationToken);

        if (inventory is null)
        {
            return;
        }


        string message = $"Product {inventory.Value.productName} at location {inventory.Value.locationName} is low on stock. Current stock: {notification.currentStock}({inventory.Value.unitOfMeasureName})";

        var notificationResponse = new NotificationResponse
        {
            Id = notification.Id,
            EventType = nameof(LowStockDomainEvent),
            Message = message,
            Title = "Low Stock Alert",
            Severity = "Warning",
            Data = new
            {
                notification.productId,
                notification.locationId,
                notification.currentStock
            },
            CreatedAt = DateTime.UtcNow,
        };
        await _notificationService.NotifyLowStockAsync(
            notificationResponse,
            cancellationToken);

    }
}
