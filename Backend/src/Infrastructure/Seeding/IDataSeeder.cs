namespace Infrastructure.Seeding;

internal interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
