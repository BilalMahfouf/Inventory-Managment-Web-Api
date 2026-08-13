using System.Text;
using FluentAssertions;
using Infrastructure.IntegrationTests.Common;
using Infrastructure.Services.PdfGeneration;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp.Media;

namespace Infrastructure.IntegrationTests.Services;

public sealed class PdfOptionsTests : PdfIntegrationTestBase
{
    public PdfOptionsTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task PortraitOrientation_GeneratesValidPdf()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body>Portrait</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.A4,
            Landscape = false,
        });

        pdf.Should().NotBeEmpty();
        Encoding.ASCII.GetString(pdf.AsSpan(0, 5)).Should().Be("%PDF-");
        await page.DisposeAsync();
    }

    [Fact]
    public async Task LandscapeOrientation_GeneratesValidPdf()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body>Landscape</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.A4,
            Landscape = true,
        });

        pdf.Should().NotBeEmpty();
        await page.DisposeAsync();
    }

    [Fact]
    public async Task CustomPaperSize_GeneratesValidPdf()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body>Letter</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.Letter,
        });

        pdf.Should().NotBeEmpty();
        await page.DisposeAsync();
    }

    [Fact]
    public async Task CustomMargins_GeneratesValidPdf()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body>Margins</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.A4,
            MarginOptions = new MarginOptions
            {
                Top = "20mm",
                Bottom = "20mm",
                Left = "15mm",
                Right = "15mm",
            },
        });

        pdf.Should().NotBeEmpty();
        await page.DisposeAsync();
    }

    [Fact]
    public async Task PrintBackground_True_GeneratesValidPdf()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body style='background:red'>Background</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
        });

        pdf.Should().NotBeEmpty();
        await page.DisposeAsync();
    }

    [Fact]
    public async Task HeaderTemplate_RendersInOutput()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body>Content</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.A4,
            DisplayHeaderFooter = true,
            HeaderTemplate = "<div style='font-size:10px;'>Header Text</div>",
        });

        pdf.Should().NotBeEmpty();
        await page.DisposeAsync();
    }

    [Fact]
    public async Task FooterTemplate_RendersInOutput()
    {
        var sut = Services.GetRequiredService<BrowserManager>();
        var page = await sut.CreatePageAsync();
        await page.SetContentAsync("<html><body>Content</body></html>");

        var pdf = await page.PdfDataAsync(new PuppeteerSharp.PdfOptions
        {
            Format = PaperFormat.A4,
            DisplayHeaderFooter = true,
            FooterTemplate = "<div style='font-size:10px;'>Page <span class='pageNumber'></span></div>",
        });

        pdf.Should().NotBeEmpty();
        await page.DisposeAsync();
    }
}
