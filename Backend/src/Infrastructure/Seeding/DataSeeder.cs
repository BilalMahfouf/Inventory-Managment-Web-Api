using Application.Shared.Contracts;
using Domain.Sales.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeding;

internal sealed class DataSeeder : IDataSeeder
{
    private readonly InventoryManagmentDBContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DataSeeder(InventoryManagmentDBContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        await SeedUserRolesAsync(now, cancellationToken);
        int adminUserId = await EnsureAdminUserAsync(now, cancellationToken);

        await SeedLocationTypesAsync(adminUserId, now, cancellationToken);
        await SeedSupplierTypesAsync(adminUserId, now, cancellationToken);
        await SeedCustomerCategoriesAsync(adminUserId, now, cancellationToken);
        await SeedUnitOfMeasuresAsync(adminUserId, now, cancellationToken);
        await SeedProductCategoriesAsync(adminUserId, now, cancellationToken);
        await SeedStockMovementTypesAsync(adminUserId, now, cancellationToken);

        await SeedLocationsAsync(adminUserId, now, cancellationToken);
        await SeedSuppliersAsync(adminUserId, now, cancellationToken);
        await SeedCustomersAsync(adminUserId, now, cancellationToken);
        await SeedProductsAsync(adminUserId, now, cancellationToken);
        await SeedInventoriesAsync(adminUserId, now, cancellationToken);
        await SeedSalesOrdersAsync(adminUserId, now, cancellationToken);
        await EnsureMinimumStockMovementsAsync(adminUserId, now, cancellationToken);
        await SeedStockTransfersAsync(adminUserId, now, cancellationToken);
    }

    private async Task SeedUserRolesAsync(DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.UserRoles.AnyAsync(cancellationToken))
        {
            return;
        }

        List<UserRole> roles =
        [
            new UserRole { Name = "Admin", Description = "System administrator", CreatedAt = now, IsDeleted = false },
            new UserRole { Name = "Manager", Description = "Operations manager", CreatedAt = now, IsDeleted = false },
            new UserRole { Name = "Sales", Description = "Sales operator", CreatedAt = now, IsDeleted = false },
            new UserRole { Name = "Warehouse", Description = "Warehouse staff", CreatedAt = now, IsDeleted = false },
            new UserRole { Name = "Viewer", Description = "Read only user", CreatedAt = now, IsDeleted = false },
        ];

        _dbContext.UserRoles.AddRange(roles);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> EnsureAdminUserAsync(DateTime now, CancellationToken cancellationToken)
    {
        const string adminEmail = "seed.admin@ims.local";

        User? existingAdmin = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.Email == adminEmail, cancellationToken);

        if (existingAdmin is not null)
        {
            return existingAdmin.Id;
        }

        int? adminRoleId = await _dbContext.UserRoles
            .Where(e => e.Name == "Admin")
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminRoleId is null)
        {
            UserRole adminRole = new()
            {
                Name = "Admin",
                Description = "System administrator",
                CreatedAt = now,
                IsDeleted = false,
            };

            _dbContext.UserRoles.Add(adminRole);
            await _dbContext.SaveChangesAsync(cancellationToken);
            adminRoleId = adminRole.Id;
        }

        User admin = new()
        {
            FirstName = "Seed",
            LastName = "Admin",
            UserName = "seed-admin",
            Email = adminEmail,
            PasswordHash = _passwordHasher.HashPassword("Admin@12345"),
            RoleId = adminRoleId.Value,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = now,
            IsDeleted = false,
        };

        _dbContext.Users.Add(admin);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return admin.Id;
    }

    private async Task SeedLocationTypesAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.LocationTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        List<LocationType> locationTypes =
        [
            new LocationType { Name = "Warehouse", Description = "Main stock storage", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new LocationType { Name = "Store", Description = "Retail point", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new LocationType { Name = "Transit", Description = "Goods in transfer", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new LocationType { Name = "Returns", Description = "Returned goods area", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
        ];

        _dbContext.LocationTypes.AddRange(locationTypes);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSupplierTypesAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.SupplierTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        List<SupplierType> supplierTypes =
        [
            new SupplierType { Name = "Manufacturer", Description = "Direct manufacturer", IsIndividual = false, CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new SupplierType { Name = "Distributor", Description = "Regional distributor", IsIndividual = false, CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new SupplierType { Name = "Individual Vendor", Description = "Independent supplier", IsIndividual = true, CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
        ];

        _dbContext.SupplierTypes.AddRange(supplierTypes);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedCustomerCategoriesAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.CustomerCategories.AnyAsync(cancellationToken))
        {
            return;
        }

        List<CustomerCategory> customerCategories =
        [
            CustomerCategory.Create("Retail", true, "Individual buyers"),
            CustomerCategory.Create("Corporate", false, "Business accounts"),
            CustomerCategory.Create("Wholesale", false, "Bulk buying customers"),
        ];

        foreach (CustomerCategory category in customerCategories)
        {
            category.CreatedByUserId = adminUserId;
            category.CreatedAt = now;
            category.IsDeleted = false;
        }

        _dbContext.CustomerCategories.AddRange(customerCategories);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedUnitOfMeasuresAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.UnitOfMeasures.AnyAsync(cancellationToken))
        {
            return;
        }

        List<UnitOfMeasure> unitOfMeasures =
        [
            new UnitOfMeasure { Name = "Piece", Description = "Single item", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new UnitOfMeasure { Name = "Box", Description = "Packed box", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new UnitOfMeasure { Name = "Kg", Description = "Kilogram", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new UnitOfMeasure { Name = "Liter", Description = "Liquid measure", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new UnitOfMeasure { Name = "Pack", Description = "Bundle pack", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
        ];

        _dbContext.UnitOfMeasures.AddRange(unitOfMeasures);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedProductCategoriesAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.ProductCategories.AnyAsync(cancellationToken))
        {
            return;
        }

        List<ProductCategory> mainCategories =
        [
            new ProductCategory { Name = "Electronics", Type = ProductCategoryType.MainCategory, Description = "Electronic products", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Food", Type = ProductCategoryType.MainCategory, Description = "Food items", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Office", Type = ProductCategoryType.MainCategory, Description = "Office essentials", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
        ];

        _dbContext.ProductCategories.AddRange(mainCategories);
        await _dbContext.SaveChangesAsync(cancellationToken);

        Dictionary<string, int> mainCategoryIds = await _dbContext.ProductCategories
            .Where(e => e.Type == ProductCategoryType.MainCategory)
            .ToDictionaryAsync(e => e.Name, e => e.Id, cancellationToken);

        List<ProductCategory> subCategories =
        [
            new ProductCategory { Name = "Mobiles", Type = ProductCategoryType.SubCategory, ParentId = mainCategoryIds["Electronics"], Description = "Smartphones", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Accessories", Type = ProductCategoryType.SubCategory, ParentId = mainCategoryIds["Electronics"], Description = "Electronic accessories", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Snacks", Type = ProductCategoryType.SubCategory, ParentId = mainCategoryIds["Food"], Description = "Snack products", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Beverages", Type = ProductCategoryType.SubCategory, ParentId = mainCategoryIds["Food"], Description = "Drink products", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Stationery", Type = ProductCategoryType.SubCategory, ParentId = mainCategoryIds["Office"], Description = "Office stationery", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
            new ProductCategory { Name = "Printing", Type = ProductCategoryType.SubCategory, ParentId = mainCategoryIds["Office"], Description = "Printing supplies", CreatedByUserId = adminUserId, CreatedAt = now, IsDeleted = false },
        ];

        _dbContext.ProductCategories.AddRange(subCategories);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedStockMovementTypesAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.StockMovementTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        var movementTypes = Enum
            .GetValues<StockMovementTypeEnum>()
            .Select(e => new StockMovementType
            {
                Name = e.ToString(),
                Direction = MapDirection(e),
                Description = $"Seeded movement type for {e}",
                CreatedByUserId = adminUserId,
                CreatedAt = now,
                IsDeleted = false,
            })
            .ToList();

        _dbContext.StockMovementTypes.AddRange(movementTypes);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedLocationsAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.Locations.AnyAsync(cancellationToken))
        {
            return;
        }

        List<int> locationTypeIds = await _dbContext.LocationTypes
            .OrderBy(e => e.Id)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Location> locations = [];
        for (int i = 0; i < 5; i++)
        {
            locations.Add(new Location
            {
                Name = $"Location-{i + 1:00}",
                Address = $"Warehouse Street {i + 1}, District {i + 1}",
                IsActive = true,
                LocationTypeId = locationTypeIds[i % locationTypeIds.Count],
                CreatedByUserId = adminUserId,
                CreatedAt = now.AddMinutes(i),
                IsDeleted = false,
            });
        }

        _dbContext.Locations.AddRange(locations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSuppliersAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.Suppliers.AnyAsync(cancellationToken))
        {
            return;
        }

        List<int> supplierTypeIds = await _dbContext.SupplierTypes
            .OrderBy(e => e.Id)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Supplier> suppliers = [];
        for (int i = 0; i < 20; i++)
        {
            suppliers.Add(new Supplier
            {
                Name = $"Supplier {i + 1:00}",
                SupplierTypeId = supplierTypeIds[i % supplierTypeIds.Count],
                Email = $"supplier{i + 1:00}@ims.local",
                Phone = $"+100000{i + 1:0000}",
                Address = $"Supplier Address {i + 1}",
                Terms = "Net 30",
                IsActive = true,
                CreatedByUserId = adminUserId,
                CreatedAt = now.AddMinutes(i),
                IsDeleted = false,
            });
        }

        _dbContext.Suppliers.AddRange(suppliers);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedCustomersAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.Customers.AnyAsync(cancellationToken))
        {
            return;
        }

        List<int> customerCategoryIds = await _dbContext.CustomerCategories
            .OrderBy(e => e.Id)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Customer> customers = [];
        for (int i = 0; i < 20; i++)
        {
            Customer customer = Customer.Create(
                $"Customer {i + 1:00}",
                customerCategoryIds[i % customerCategoryIds.Count],
                $"customer{i + 1:00}@ims.local",
                $"+200000{i + 1:0000}",
                Address.Create(
                    $"Street {i + 1}",
                    "Amman",
                    "Amman",
                    $"11{i + 1:000}"));

            customer.CreatedByUserId = adminUserId;
            customer.CreatedAt = now.AddMinutes(i);
            customer.IsDeleted = false;
            customers.Add(customer);
        }

        _dbContext.Customers.AddRange(customers);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedProductsAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        List<int> categoryIds = await _dbContext.ProductCategories
            .Where(e => e.Type == ProductCategoryType.SubCategory)
            .OrderBy(e => e.Id)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        if (categoryIds.Count == 0)
        {
            categoryIds = await _dbContext.ProductCategories
                .OrderBy(e => e.Id)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);
        }

        List<int> uomIds = await _dbContext.UnitOfMeasures
            .OrderBy(e => e.Id)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Product> products = [];
        for (int i = 0; i < 20; i++)
        {
            decimal cost = 5 + i;
            products.Add(new Product
            {
                Sku = $"SKU-IMS-{i + 1:000}",
                Name = $"Product {i + 1:00}",
                Description = $"Seeded product number {i + 1}",
                CategoryId = categoryIds[i % categoryIds.Count],
                UnitOfMeasureId = uomIds[i % uomIds.Count],
                Cost = cost,
                UnitPrice = cost + 3,
                IsActive = true,
                CreatedByUserId = adminUserId,
                CreatedAt = now.AddMinutes(i),
                IsDeleted = false,
            });
        }

        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedInventoriesAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.Inventories.AnyAsync(cancellationToken))
        {
            return;
        }

        List<Product> products = await _dbContext.Products
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Location> locations = await _dbContext.Locations
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Inventory> inventories = [];
        for (int i = 0; i < 20; i++)
        {
            Product product = products[i % products.Count];
            Location location = locations[i % locations.Count];

            inventories.Add(new Inventory
            {
                ProductId = product.Id,
                LocationId = location.Id,
                QuantityOnHand = 120 + i,
                ReorderLevel = 15,
                MaxLevel = 300,
                CreatedByUserId = adminUserId,
                CreatedAt = now.AddMinutes(i),
                IsDeleted = false,
            });
        }

        _dbContext.Inventories.AddRange(inventories);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSalesOrdersAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.SalesOrders.AnyAsync(cancellationToken))
        {
            return;
        }

        List<Customer> customers = await _dbContext.Customers
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Inventory> inventories = await _dbContext.Inventories
            .Include(e => e.Product)
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        List<SalesOrder> orders = [];
        for (int i = 0; i < 20; i++)
        {
            Inventory inventory = inventories[i % inventories.Count];

            SalesOrderItemRequest itemRequest = new()
            {
                InventoryId = inventory.Id,
                Inventory = inventory,
                Product = inventory.Product,
                Quantity = (i % 3) + 1,
            };

            SalesOrder order = SalesOrder.Create(
                customers[i % customers.Count].Id,
                [itemRequest],
                $"Seed sales order {i + 1:00}",
                $"Delivery street {i + 1:00}");

            order.CreatedByUserId = adminUserId;
            order.CreatedAt = now.AddHours(i);
            order.IsDeleted = false;

            foreach (var orderItem in order.Items)
            {
                orderItem.CreatedByUserId = adminUserId;
                orderItem.CreatedAt = order.CreatedAt;
                orderItem.IsDeleted = false;
            }

            StampGeneratedStockMovements(inventory, adminUserId, order.CreatedAt);
            orders.Add(order);
        }

        _dbContext.SalesOrders.AddRange(orders);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureMinimumStockMovementsAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        int stockMovementCount = await _dbContext.StockMovements.CountAsync(cancellationToken);
        if (stockMovementCount >= 20)
        {
            return;
        }

        List<Inventory> inventories = await _dbContext.Inventories
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        int missing = 20 - stockMovementCount;
        List<StockMovement> movementFallback = [];

        for (int i = 0; i < missing; i++)
        {
            Inventory inventory = inventories[i % inventories.Count];
            movementFallback.Add(new StockMovement
            {
                ProductId = inventory.ProductId,
                InventoryId = inventory.Id,
                MovementTypeId = (int)StockMovementTypeEnum.StockIncreaseAdjustment,
                Quantity = 2 + i,
                StockMovmentStatus = StockMovementStatus.Completed,
                Notes = "Seed fallback stock movement",
                CreatedByUserId = adminUserId,
                CreatedAt = now.AddMinutes(i),
                IsDeleted = false,
            });
        }

        _dbContext.StockMovements.AddRange(movementFallback);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedStockTransfersAsync(int adminUserId, DateTime now, CancellationToken cancellationToken)
    {
        if (await _dbContext.StockTransfers.AnyAsync(cancellationToken))
        {
            return;
        }

        List<Product> products = await _dbContext.Products
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        List<Location> locations = await _dbContext.Locations
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

        if (locations.Count < 2)
        {
            return;
        }

        List<StockTransfer> transfers = [];
        for (int i = 0; i < 20; i++)
        {
            Product product = products[i % products.Count];
            Location fromLocation = locations[i % locations.Count];
            Location toLocation = locations[(i + 1) % locations.Count];

            StockTransfer transfer = StockTransfer.Create(
                product.Id,
                fromLocation.Id,
                toLocation.Id,
                (i % 5) + 1);

            transfer.CreatedByUserId = adminUserId;
            transfer.CreatedAt = now.AddMinutes(i);
            transfer.IsDeleted = false;

            transfers.Add(transfer);
        }

        _dbContext.StockTransfers.AddRange(transfers);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static StockMovementDirection MapDirection(StockMovementTypeEnum movementType)
    {
        return movementType switch
        {
            StockMovementTypeEnum.PurchaseReceipt => StockMovementDirection.In,
            StockMovementTypeEnum.SalesOrder => StockMovementDirection.Out,
            StockMovementTypeEnum.StockAdjustment => StockMovementDirection.Transfer,
            StockMovementTypeEnum.TransferIn => StockMovementDirection.In,
            StockMovementTypeEnum.TransferOut => StockMovementDirection.Out,
            StockMovementTypeEnum.ReturnFromCustomer => StockMovementDirection.In,
            StockMovementTypeEnum.ReturnToSupplier => StockMovementDirection.Out,
            StockMovementTypeEnum.ManufacturingUse => StockMovementDirection.Out,
            StockMovementTypeEnum.ProductionOutput => StockMovementDirection.In,
            StockMovementTypeEnum.CycleCountAdjustment => StockMovementDirection.Transfer,
            StockMovementTypeEnum.DamageLoss => StockMovementDirection.Out,
            StockMovementTypeEnum.PromotionalSample => StockMovementDirection.Out,
            StockMovementTypeEnum.InitialStock => StockMovementDirection.In,
            StockMovementTypeEnum.StockDecreaseAdjustment => StockMovementDirection.Out,
            StockMovementTypeEnum.StockIncreaseAdjustment => StockMovementDirection.In,
            _ => StockMovementDirection.Transfer,
        };
    }

    private static void StampGeneratedStockMovements(Inventory inventory, int adminUserId, DateTime createdAt)
    {
        foreach (StockMovement movement in inventory.StockMovements)
        {
            if (movement.CreatedByUserId <= 0)
            {
                movement.CreatedByUserId = adminUserId;
            }

            if (movement.CreatedAt == default)
            {
                movement.CreatedAt = createdAt;
            }

            if (movement.StockMovmentStatus == 0)
            {
                movement.StockMovmentStatus = StockMovementStatus.Pending;
            }

            movement.IsDeleted = false;
        }
    }
}
