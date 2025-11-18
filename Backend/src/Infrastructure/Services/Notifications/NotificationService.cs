using Application.Abstractions;
using Application.DTOs.Notifications;
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
        NotificationResponse response,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync("test", new
        {
            Message = "This is a test message"
        }, cancellationToken);

        await _hubContext.Clients.All.SendAsync("low-stock-alert",
           (object)response,
           cancellationToken);
    }
}
