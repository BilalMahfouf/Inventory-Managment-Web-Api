using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using PuppeteerSharp;

namespace Infrastructure.IntegrationTests.Common;

public sealed class PdfTestWebAppFactory : WebApplicationFactory<Presentation.Endpoints.Sales.SalesOrderEndpoints>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../src/Presentation"));
        builder.UseContentRoot(contentRoot);
        builder.UseEnvironment("Testing");
        SetRequiredEnvironmentVariables();
    }

    public async Task InitializeAsync()
    {
        await new BrowserFetcher().DownloadAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
    }

    private static void SetRequiredEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable(
            "DefaultConnectionPgSql",
            Environment.GetEnvironmentVariable("IMS_TEST_CONNECTION_STRING")
            ?? "Host=localhost;Database=integration_tests_pdf;Username=postgres;Password=IntegrationTests_123!");
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "integration-tests-jwt-secret-key-1234567890");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "integration-tests");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "integration-tests");
        Environment.SetEnvironmentVariable("JWT_ACCESS_TOKEN_LIFETIME_MINUTES", "30");
        Environment.SetEnvironmentVariable("EMAIL_CONFIGURATIONS_PASSWORD", "integration-tests-password");
        Environment.SetEnvironmentVariable("EMAIL_CONFIGURATIONS_EMAIL", "integration-tests@ims.local");
    }
}
