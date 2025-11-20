using Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Inventories;


public sealed record LowStockDomainEvent(
    int productId,
    int locationId,
    decimal currentStock)
    : DomainEvent();
