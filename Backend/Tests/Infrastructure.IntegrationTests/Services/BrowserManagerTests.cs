using FluentAssertions;
using Infrastructure.IntegrationTests.Common;
using Infrastructure.Services.PdfGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IntegrationTests.Services;

public sealed class BrowserManagerTests : PdfIntegrationTestBase
{
    public BrowserManagerTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreatePageAsync_ReturnsNonNullPage()
    {
        var sut = Services.GetRequiredService<BrowserManager>();

        var page = await sut.CreatePageAsync();

        page.Should().NotBeNull();
        await page.DisposeAsync();
    }

    [Fact]
    public async Task CreatePageAsync_CanCreateMultiplePages()
    {
        var sut = Services.GetRequiredService<BrowserManager>();

        var page1 = await sut.CreatePageAsync();
        var page2 = await sut.CreatePageAsync();

        page1.Should().NotBeNull();
        page2.Should().NotBeNull();
        page1.Should().NotBeSameAs(page2);

        await page1.DisposeAsync();
        await page2.DisposeAsync();
    }

    [Fact]
    public async Task CreatePageAsync_BrowserIsReusedAcrossCalls()
    {
        var sut = Services.GetRequiredService<BrowserManager>();

        var page1 = await sut.CreatePageAsync();
        var page2 = await sut.CreatePageAsync();

        var browser1 = page1.Browser;
        var browser2 = page2.Browser;

        browser1.Should().BeSameAs(browser2);

        await page1.DisposeAsync();
        await page2.DisposeAsync();
    }

    [Fact]
    public async Task CreatePageAsync_AfterDispose_StartsNewBrowser()
    {
        var sut = new BrowserManager();

        var page = await sut.CreatePageAsync();
        await page.DisposeAsync();
        await sut.DisposeAsync();

        var newPage = await sut.CreatePageAsync();
        newPage.Should().NotBeNull();
        await newPage.DisposeAsync();
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task DisposalOfPage_DoesNotDisposeBrowser()
    {
        var sut = Services.GetRequiredService<BrowserManager>();

        var page1 = await sut.CreatePageAsync();
        await page1.DisposeAsync();

        var page2 = await sut.CreatePageAsync();
        page2.Should().NotBeNull();
        await page2.DisposeAsync();
    }
}
