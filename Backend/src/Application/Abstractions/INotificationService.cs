using Application.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions;

public interface  INotificationService
{
    Task NotifyLowStockAsync( 
        NotificationResponse response,
        CancellationToken cancellationToken = default);
}
