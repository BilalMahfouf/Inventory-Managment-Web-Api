using System.Text;
using Application.PdfGeneration;
using FluentAssertions;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;

namespace Infrastructure.IntegrationTests.Services;

public sealed class PdfGeneratorTests : PdfIntegrationTestBase
{
    public PdfGeneratorTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GenerateAsync_ValidSalesOrder_ReturnsNonEmptyBytes()
    {
        var model = CreateValidSalesOrderModel();

        var result = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GenerateAsync_ValidSalesOrder_ReturnsPdfWithCorrectSignature()
    {
        var model = CreateValidSalesOrderModel();

        var result = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);

        var header = Encoding.ASCII.GetString(result.AsSpan(0, Math.Min(5, result.Length)));
        header.Should().Be("%PDF-");
    }

    [Fact]
    public async Task GenerateAsync_SameTemplateDifferentModels_ProducesDifferentContent()
    {
        var model1 = CreateValidSalesOrderModel() with { OrderId = 1, CustomerName = "First" };
        var model2 = CreateValidSalesOrderModel() with { OrderId = 2, CustomerName = "Second" };

        var result1 = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model1);
        var result2 = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model2);

        result1.Should().NotBeSameAs(result2);
        result1.Should().NotBeEquivalentTo(result2);
    }

    private static SalesOrderPdfModel CreateValidSalesOrderModel()
    {
        return new SalesOrderPdfModel(
            OrderId: 1001,
            CustomerName: "Test Customer",
            OrderDate: "2026-07-30",
            Status: "Pending",
            Items:
            [
                new SalesOrderItemPdfModel("Widget A", 5, 12.50m, 62.50m),
                new SalesOrderItemPdfModel("Gadget B", 3, 24.00m, 72.00m),
            ],
            TotalAmount: 134.50m);
    }
}
