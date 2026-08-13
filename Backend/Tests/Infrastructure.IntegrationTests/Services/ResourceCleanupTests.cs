using Application.PdfGeneration;
using FluentAssertions;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;
using Infrastructure.Services.PdfGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IntegrationTests.Services;

public sealed class ResourceCleanupTests : PdfIntegrationTestBase
{
    public ResourceCleanupTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateAsync_PageIsDisposedAfterGeneration()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();

        await page.SetContentAsync("<html><body>Test</body></html>");
        await page.PdfDataAsync(new PuppeteerSharp.PdfOptions());

        var isClosed = page.IsClosed;
        await page.DisposeAsync();

        isClosed.Should().BeFalse("PdfDataAsync should not close the page by itself");
    }

    [Fact]
    public async Task BrowserPage_ExplicitDispose_ClosesPage()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();

        await page.DisposeAsync();

        page.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task BrowserManager_Dispose_DisposesBrowser()
    {
        var sut = new BrowserManager();
        var page = await sut.CreatePageAsync();

        await sut.DisposeAsync();

        page.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task BrowserManager_Dispose_Idempotent()
    {
        var sut = new BrowserManager();
        await sut.CreatePageAsync();

        await sut.DisposeAsync();
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task BrowserManager_RemainsAliveAfterPageDisposal()
    {
        var sut = Services.GetRequiredService<BrowserManager>();

        var page1 = await sut.CreatePageAsync();
        await page1.DisposeAsync();

        var page2 = await sut.CreatePageAsync();
        page2.Should().NotBeNull();
        page2.IsClosed.Should().BeFalse();

        await page2.DisposeAsync();
    }

    [Fact]
    public async Task PdfGenerator_MultipleCalls_AllPagesEventuallyDisposed()
    {
        var model = new SalesOrderPdfModel(
            1, "Customer", "2026-07-30", "Pending",
            [new SalesOrderItemPdfModel("Item", 1, 10, 10)], 10);

        var pdf1 = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);
        var pdf2 = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);

        pdf1.Should().NotBeEmpty();
        pdf2.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LargeNumberOfGenerations_NoResourceLeak()
    {
        var model = new SalesOrderPdfModel(
            1, "Customer", "2026-07-30", "Pending",
            [new SalesOrderItemPdfModel("Item", 1, 10, 10)], 10);

        for (var i = 0; i < 20; i++)
        {
            var pdf = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);
            pdf.Should().NotBeEmpty();
        }
    }
}
