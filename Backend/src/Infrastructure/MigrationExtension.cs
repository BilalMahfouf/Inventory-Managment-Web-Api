using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;


public static class MigrationExtension
{
    /// <summary>
    /// Applies any pending EF Core migrations to the database on application startup.
    /// Creates a scoped <see cref="ApplicationDbContext"/> to run <c>Database.Migrate()</c>.
    /// </summary>
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using InventoryManagmentDBContext dbContext =
            scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();

        dbContext.Database.Migrate();

    }

}
