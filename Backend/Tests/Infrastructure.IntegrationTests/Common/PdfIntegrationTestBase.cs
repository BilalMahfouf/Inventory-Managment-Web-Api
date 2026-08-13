using Application.PdfGeneration.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IntegrationTests.Common;

public abstract class PdfIntegrationTestBase : IClassFixture<PdfTestWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _scope;

    protected PdfIntegrationTestBase(PdfTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();
        PdfGenerator = _scope.ServiceProvider.GetRequiredService<IPdfGenerator>();
    }

    protected IPdfGenerator PdfGenerator { get; }

    protected IServiceProvider Services => _scope.ServiceProvider;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _scope.DisposeAsync();
    }
}
