using Application.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.Contracts;

public interface  INotificationService
{
    Task NotifyLowStockAsync( 
        NotificationResponse response,
        CancellationToken cancellationToken = default);
}
