using Application.Abstractions;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Notifications;

internal class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyLowStockAsync(
        int productId,
        int locationId, 
        decimal currentStock,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("low-stock-alert",
           new
           {
               ProductId = productId,
               LocationId = locationId,
               CurrentStock = currentStock
           }, cancellationToken);
    }
}
