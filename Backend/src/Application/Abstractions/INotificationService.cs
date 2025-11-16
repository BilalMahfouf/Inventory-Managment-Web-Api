using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Abstractions;

public interface  INotificationService
{
    Task NotifyLowStockAsync(
        int productId,
        int locationId,
        decimal currentStock,
        CancellationToken cancellationToken = default);
}
