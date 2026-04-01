namespace Domain.Sales.Enums;

public enum SalesOrderStatus : byte
{
    Pending = 1,
    Confirmed = 2,
    InTransit = 3,
    Shipped = 4,
    Completed = 5,
    Cancelled = 6,
    Returned = 7,
}

public enum PaymentStatus : byte
{
    Unpaid = 1,
    PartiallyPaid = 2,
    Paid = 3,
}
