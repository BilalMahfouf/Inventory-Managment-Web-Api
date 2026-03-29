namespace Domain.Shared.Errors;

public static class DomainErrors
{
    public static class Users
    {
        public static Error NotFound(string value) => Error.NotFound("User.NotFound", $"User '{value}' was not found.");
        public static Error InvalidCredentials => Error.Unauthorized("User.InvalidCredentials", "The provided credentials are invalid.");
        public static Error InvalidPassword => Error.Validation("User.InvalidPassword", "The provided password is invalid.");
        public static Error EmailAlreadyInUse(string email) => Error.Conflict("User.EmailAlreadyInUse", $"Email '{email}' is already in use.");
    }

    public static class Authentication
    {
        public static Error InvalidToken => Error.Validation("Authentication.InvalidToken", "The token is invalid.");
        public static Error ExpiredToken => Error.Validation("Authentication.ExpiredToken", "The token is expired.");
        public static Error SessionCreationFailed => Error.Failure("Authentication.SessionCreationFailed", "Cannot create user session.");
    }

    public static class Products
    {
        public static Error NotFound(string value) => Error.NotFound("Product.NotFound", $"Product '{value}' was not found.");
        public static Error SkuAlreadyExists(string sku) => Error.Conflict("Product.SkuAlreadyExists", $"SKU '{sku}' already exists.");
        public static Error InvalidPricing => Error.Conflict("Product.InvalidPricing", "Unit price cannot be less than cost.");
    }

    public static class Inventory
    {
        public static Error NotFound(string value) => Error.NotFound("Inventory.NotFound", $"Inventory '{value}' was not found.");
        public static Error InsufficientStock => Error.Conflict("Inventory.InsufficientStock", "Insufficient stock for this operation.");
    }

    public static class Customers
    {
        public static Error NotFound(string value) => Error.NotFound("Customer.NotFound", $"Customer '{value}' was not found.");
    }

    public static class Locations
    {
        public static Error NotFound(string value) => Error.NotFound("Location.NotFound", $"Location '{value}' was not found.");
    }

    public static class Sales
    {
        public static Error OrderNotFound(string value) => Error.NotFound("SalesOrder.NotFound", $"Sales order '{value}' was not found.");
    }

    public static class StockMovements
    {
        public static Error TransferNotFound(string value) => Error.NotFound("StockTransfer.NotFound", $"Stock transfer '{value}' was not found.");
    }

    public static class UnitOfMeasures
    {
        public static Error NotFound(string value) => Error.NotFound("UnitOfMeasure.NotFound", $"Unit of measure '{value}' was not found.");
    }

    public static class Dashboard
    {
        public static Error NoData => Error.NotFound("Dashboard.NoData", "No dashboard data was found.");
    }

    public static class Images
    {
        public static Error NotFound(string value) => Error.NotFound("Image.NotFound", $"Image resource '{value}' was not found.");
    }
}
