using Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sales;

public sealed record SalesOrderItemRequest(Product Product, int Quantity);
