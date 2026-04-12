using Application.Users.Contracts;
using Domain.Inventories.Entities;
using Domain.Inventories.Enums;
using Domain.Products.Entities;
using Domain.Products.Enums;
using Domain.Users.Entities;
using Infrastructure.Interceptors;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Application.IntegrationTests.Common;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Presentation.Endpoints.Sales.SalesOrderEndpoints>, IAsyncLifetime
{
    private static int _seedUserId = -1;
    private const string TestDatabaseName = "InventoryIntegrationTests";

    public const string DefaultUserRoleName = "IntegrationTestsRole";
    public const string DefaultUserEmail = "integration-tests-user@ims.local";
    public const string DefaultLocationTypeName = "Integration Warehouse Type";
    public const string DefaultLocationName = "Integration Main Location";
    public const string DefaultProductCategoryName = "Integration Default Category";
    public const string DefaultUnitOfMeasureName = "pcs";

    public static int SeedUserId => _seedUserId;

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase(TestDatabaseName)
        .WithUsername("postgres")
        .WithPassword("IntegrationTests_123!")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/Presentation"));
        builder.UseContentRoot(contentRoot);
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var connectionString = BuildTestConnectionString();
            SetRequiredEnvironmentVariables(connectionString);

            services.RemoveAll(typeof(DbContextOptions<InventoryManagmentDBContext>));
            services.RemoveAll(typeof(InventoryManagmentDBContext));
            services.RemoveAll(typeof(ICurrentUserService));

            services.AddScoped<ICurrentUserService, TestCurrentUserService>();
            services.AddDbContext<InventoryManagmentDBContext>((sp, options) => options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptors>()));
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        var connectionString = BuildTestConnectionString();
        SetRequiredEnvironmentVariables(connectionString);

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex) when (IsBrokenMigrationChain(ex))
        {
            // Fallback for legacy migration chains that cannot initialize a fresh database.
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        await SeedReferenceDataAsync(context);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    private static void SetRequiredEnvironmentVariables(string connectionString)
    {
        Environment.SetEnvironmentVariable("DefaultConnectionPgSql", connectionString);
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "integration-tests-jwt-secret-key-1234567890");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "integration-tests");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "integration-tests");
        Environment.SetEnvironmentVariable("JWT_ACCESS_TOKEN_LIFETIME_MINUTES", "30");
        Environment.SetEnvironmentVariable("EMAIL_CONFIGURATIONS_PASSWORD", "integration-tests-password");
        Environment.SetEnvironmentVariable("EMAIL_CONFIGURATIONS_EMAIL", "integration-tests@ims.local");
    }

    private string BuildTestConnectionString()
    {
        return _postgreSqlContainer.GetConnectionString();
    }

    private static async Task SeedReferenceDataAsync(InventoryManagmentDBContext context)
    {
        var role = await context.UserRoles
            .FirstOrDefaultAsync(e => e.Name == DefaultUserRoleName);

        if (role is null)
        {
            role = new UserRole
            {
                Name = DefaultUserRoleName,
                Description = "Role used by integration tests",
            };

            context.UserRoles.Add(role);
            await context.SaveChangesAsync();
        }

        var user = await context.Users
            .FirstOrDefaultAsync(e => e.Email == DefaultUserEmail);

        if (user is null)
        {
            user = new User
            {
                Email = DefaultUserEmail,
                UserName = "integration-tests",
                FirstName = "Integration",
                LastName = "Tests",
                PasswordHash = "integration-tests-password-hash",
                RoleId = role.Id,
                IsActive = true,
                EmailConfirmed = true,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        _seedUserId = user.Id;

        await EnsureStockMovementTypesAsync(context, user.Id);

        var locationType = await context.LocationTypes
            .FirstOrDefaultAsync(e => e.Name == DefaultLocationTypeName);

        if (locationType is null)
        {
            locationType = new LocationType
            {
                Name = DefaultLocationTypeName,
                Description = "Location type for integration tests",
                CreatedByUserId = user.Id,
            };

            context.LocationTypes.Add(locationType);
            await context.SaveChangesAsync();
        }

        if (!await context.Locations.AnyAsync(e => e.Name == DefaultLocationName))
        {
            context.Locations.Add(new Location
            {
                Name = DefaultLocationName,
                Address = "Integration Test Address",
                IsActive = true,
                LocationTypeId = locationType.Id,
                CreatedByUserId = user.Id,
            });
        }

        if (!await context.ProductCategories.AnyAsync(e => e.Name == DefaultProductCategoryName))
        {
            context.ProductCategories.Add(new ProductCategory
            {
                Name = DefaultProductCategoryName,
                Type = ProductCategoryType.MainCategory,
                Description = "Product category for integration tests",
                CreatedByUserId = user.Id,
            });
        }

        if (!await context.UnitOfMeasures.AnyAsync(e => e.Name == DefaultUnitOfMeasureName))
        {
            context.UnitOfMeasures.Add(new UnitOfMeasure
            {
                Name = DefaultUnitOfMeasureName,
                Description = "UoM for integration tests",
                CreatedByUserId = user.Id,
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureStockMovementTypesAsync(InventoryManagmentDBContext context, int userId)
    {
        if (await context.StockMovementTypes.AnyAsync())
        {
            return;
        }

        context.StockMovementTypes.AddRange(
            CreateStockMovementType("PurchaseReceipt", StockMovementDirection.In, userId),
            CreateStockMovementType("SalesOrder", StockMovementDirection.Out, userId),
            CreateStockMovementType("StockAdjustment", StockMovementDirection.Transfer, userId),
            CreateStockMovementType("TransferIn", StockMovementDirection.In, userId),
            CreateStockMovementType("TransferOut", StockMovementDirection.Out, userId),
            CreateStockMovementType("ReturnFromCustomer", StockMovementDirection.In, userId),
            CreateStockMovementType("ReturnToSupplier", StockMovementDirection.Out, userId),
            CreateStockMovementType("ManufacturingUse", StockMovementDirection.Out, userId),
            CreateStockMovementType("ProductionOutput", StockMovementDirection.In, userId),
            CreateStockMovementType("CycleCountAdjustment", StockMovementDirection.Transfer, userId),
            CreateStockMovementType("DamageLoss", StockMovementDirection.Out, userId),
            CreateStockMovementType("PromotionalSample", StockMovementDirection.Out, userId),
            CreateStockMovementType("InitialStock", StockMovementDirection.In, userId),
            CreateStockMovementType("StockDecreaseAdjustment", StockMovementDirection.Out, userId),
            CreateStockMovementType("StockIncreaseAdjustment", StockMovementDirection.In, userId));

        await context.SaveChangesAsync();
    }

    private static StockMovementType CreateStockMovementType(
        string name,
        StockMovementDirection direction,
        int userId)
    {
        return new StockMovementType
        {
            Name = name,
            Direction = direction,
            Description = $"Integration seed for {name}",
            CreatedByUserId = userId,
        };
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public int UserId => _seedUserId;
    }

    private static bool IsBrokenMigrationChain(Exception ex)
    {
        return ContainsBrokenMigrationMessage(ex);
    }

    private static bool ContainsBrokenMigrationMessage(Exception ex)
    {
        if (ex.Message.Contains("Cannot find the object", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("relation", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return ex.InnerException is not null && ContainsBrokenMigrationMessage(ex.InnerException);
    }
}
