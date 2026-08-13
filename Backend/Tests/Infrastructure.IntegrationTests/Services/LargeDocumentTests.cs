using System.Text;
using Application.PdfGeneration;
using FluentAssertions;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;

namespace Infrastructure.IntegrationTests.Services;

public sealed class LargeDocumentTests : PdfIntegrationTestBase
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    public LargeDocumentTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateAsync_LargeModel_CompletesSuccessfully()
    {
        var model = CreateLargeModel(itemCount: 1_000);

        var result = await PdfGenerator
            .GenerateAsync(PdfTemplate.SalesOrder, model)
            .WaitAsync(Timeout);

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GenerateAsync_LargeModel_OutputIsValidPdf()
    {
        var model = CreateLargeModel(itemCount: 500);

        var result = await PdfGenerator
            .GenerateAsync(PdfTemplate.SalesOrder, model)
            .WaitAsync(Timeout);

        var header = Encoding.ASCII.GetString(result.AsSpan(0, 5));
        header.Should().Be("%PDF-");
    }

    [Fact]
    public async Task GenerateAsync_LargeModel_CompletesWithinTimeout()
    {
        var model = CreateLargeModel(itemCount: 2_000);

        var act = () => PdfGenerator
            .GenerateAsync(PdfTemplate.SalesOrder, model)
            .WaitAsync(Timeout);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GenerateAsync_MultipleLargeDocuments_Sequential()
    {
        var model1 = CreateLargeModel(itemCount: 300);
        var model2 = CreateLargeModel(itemCount: 300);

        var result1 = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model1);
        var result2 = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model2);

        result1.Should().NotBeEmpty();
        result2.Should().NotBeEmpty();
    }

    private static SalesOrderPdfModel CreateLargeModel(int itemCount)
    {
        var items = Enumerable.Range(1, itemCount)
            .Select(i => new SalesOrderItemPdfModel(
                $"Product {i}",
                i % 10 + 1,
                i % 50 + 0.99m,
                (i % 10 + 1) * (i % 50 + 0.99m)))
            .ToList();

        return new SalesOrderPdfModel(
            9999,
            "Large Order Customer",
            "2026-07-30",
            "Pending",
            items,
            items.Sum(i => i.TotalPrice));
    }
}
