using Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Domain.Sales;

public sealed record SalesOrderItemRequest()
{
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
}
