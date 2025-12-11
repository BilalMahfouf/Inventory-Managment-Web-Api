using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.RequestResponse;

public sealed record SalesOrderItemRequest(int ProductId,decimal Quantity);
