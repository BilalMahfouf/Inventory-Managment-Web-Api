using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Inventories;


public sealed record LowStockNotificationDetails(
    string productName,
    string locationName,
    string unitOfMeasureName);
