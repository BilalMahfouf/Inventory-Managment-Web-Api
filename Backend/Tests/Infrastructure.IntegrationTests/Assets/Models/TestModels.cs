namespace Infrastructure.IntegrationTests.Assets.Models;

public sealed record SimpleModel(string Name);

public sealed record NestedModel(string Name, AddressModel Address);

public sealed record AddressModel(string City, string Street);

public sealed record CollectionModel(string Title, List<ItemModel> Items);

public sealed record ItemModel(string Name, decimal Price);

public sealed record ConditionalModel(bool ShowDetails, string Message);

public sealed record SalesOrderPdfModel(
    int OrderId,
    string CustomerName,
    string OrderDate,
    string Status,
    List<SalesOrderItemPdfModel> Items,
    decimal TotalAmount);

public sealed record SalesOrderItemPdfModel(
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
