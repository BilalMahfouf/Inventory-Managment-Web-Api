using System.Text;
using Application.PdfGeneration;
using FluentAssertions;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;

namespace Infrastructure.IntegrationTests.Services;

public sealed class ConcurrencyTests : PdfIntegrationTestBase
{
    public ConcurrencyTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateAsync_MultipleConcurrentCalls_AllSucceed()
    {
        const int concurrencyLevel = 10;
        var models = Enumerable.Range(1, concurrencyLevel)
            .Select(i => CreateModel(i, $"Customer {i}"))
            .ToList();

        var tasks = models.Select(m => PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, m));
        var results = await Task.WhenAll(tasks);

        results.Should().HaveCount(concurrencyLevel);
        results.Should().AllSatisfy(pdf => pdf.Should().NotBeEmpty());
    }

    [Fact]
    public async Task GenerateAsync_ConcurrentCalls_ProduceDistinctOutput()
    {
        var model1 = CreateModel(1, "First");
        var model2 = CreateModel(2, "Second");

        var task1 = PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model1);
        var task2 = PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model2);

        var results = await Task.WhenAll(task1, task2);

        results[0].Should().NotBeEquivalentTo(results[1]);
    }

    [Fact]
    public async Task GenerateAsync_ConcurrentCalls_AllProduceValidPdfSignature()
    {
        const int concurrencyLevel = 5;
        var models = Enumerable.Range(1, concurrencyLevel)
            .Select(i => CreateModel(i, $"Customer {i}"))
            .ToList();

        var tasks = models.Select(m => PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, m));
        var results = await Task.WhenAll(tasks);

        results.Should().AllSatisfy(pdf =>
        {
            var header = Encoding.ASCII.GetString(pdf.AsSpan(0, 5));
            header.Should().Be("%PDF-");
        });
    }

    [Fact]
    public async Task GenerateAsync_ConcurrentCalls_NoCorruptedOutput()
    {
        const int concurrencyLevel = 8;
        var models = Enumerable.Range(1, concurrencyLevel)
            .Select(i => CreateModel(i, $"Customer {i}"))
            .ToList();

        var tasks = models.Select(m => PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, m));
        var results = await Task.WhenAll(tasks);

        foreach (var pdf in results)
        {
            var header = Encoding.ASCII.GetString(pdf.AsSpan(0, Math.Min(8, pdf.Length)));
            header.Should().StartWith("%PDF-");
            var trailerIndex = pdf.Length - 7;
            if (trailerIndex > 0)
            {
                var trailer = Encoding.ASCII.GetString(pdf.AsSpan(trailerIndex));
                trailer.Should().Contain("%%EOF");
            }
        }
    }

    private static SalesOrderPdfModel CreateModel(int id, string customer)
    {
        return new SalesOrderPdfModel(
            id, customer, "2026-07-30", "Pending",
            [new SalesOrderItemPdfModel($"Product {id}", id, 10.00m, id * 10.00m)],
            id * 10.00m);
    }
}
