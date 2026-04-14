using Infrastructure.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InitDataExtension
{
    public static void InitData(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IDataSeeder seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        seeder.SeedAsync().GetAwaiter().GetResult();
    }
}
