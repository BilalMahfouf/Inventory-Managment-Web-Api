using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions;
using Application.DTOs.Notifications;
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
        if (notification is null)
        {
            return;
        }
        var notificationResponse = new NotificationResponse
        {
            Id = notification.Id,
            EventType = nameof(LowStockDomainEvent),
            Message = $"Product {notification.productId} at location {notification.locationId} is low on stock. Current stock: {notification.currentStock}",
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
        Console.WriteLine(
            $"LowStockDomainEvent handled for ProductId: {notificationResponse.ToString()}, LocationId: {notification.locationId}, CurrentStock: {notification.currentStock}");

    }
}
