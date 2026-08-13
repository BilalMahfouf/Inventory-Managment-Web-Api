using Application.PdfGeneration;
using FluentAssertions;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;
using Infrastructure.Services.PdfGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IntegrationTests.Services;

public sealed class ErrorHandlingTests : PdfIntegrationTestBase
{
    public ErrorHandlingTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateAsync_InvalidPdfTemplate_ThrowsInvalidOperationException()
    {
        var invalidTemplate = (PdfTemplate)999;
        var model = new SimpleModel("Test");

        var act = () => PdfGenerator.GenerateAsync(invalidTemplate, model);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Template resource not found*");
    }

    [Fact]
    public async Task GenerateAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        var model = CreateSalesOrderModel();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task BrowserManager_DisposedAndUsed_Throws()
    {
        var sut = new BrowserManager();
        await sut.DisposeAsync();

        var act = () => sut.CreatePageAsync();

        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    [Fact]
    public async Task PdfGenerator_DisposedAndUsed_Throws()
    {
        var sut = Services.GetRequiredService<PdfGenerator>();

        await sut.DisposeAsync();

        var model = new SimpleModel("Test");
        var act = () => sut.GenerateAsync(PdfTemplate.SalesOrder, model);

        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    private static SalesOrderPdfModel CreateSalesOrderModel()
    {
        return new SalesOrderPdfModel(
            1, "Customer", "2026-07-30", "Pending",
            [new SalesOrderItemPdfModel("Item", 1, 10, 10)], 10);
    }
}
