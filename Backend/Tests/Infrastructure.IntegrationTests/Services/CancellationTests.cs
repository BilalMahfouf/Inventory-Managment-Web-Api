using Application.PdfGeneration;
using FluentAssertions;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;
using Infrastructure.Services.PdfGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IntegrationTests.Services;

public sealed class CancellationTests : PdfIntegrationTestBase
{
    public CancellationTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateAsync_CancelledBeforeStart_ThrowsOperationCanceledException()
    {
        var model = CreateModel();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task CreatePageAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => sut.CreatePageAsync(cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    private static SalesOrderPdfModel CreateModel()
    {
        return new SalesOrderPdfModel(
            1, "Customer", "2026-07-30", "Pending",
            [new SalesOrderItemPdfModel("Item", 1, 10, 10)], 10);
    }
}
