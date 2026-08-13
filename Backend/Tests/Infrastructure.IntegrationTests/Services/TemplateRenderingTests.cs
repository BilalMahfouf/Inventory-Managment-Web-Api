using Application.PdfGeneration;
using FluentAssertions;
using HandlebarsDotNet;
using Infrastructure.IntegrationTests.Assets.Models;
using Infrastructure.IntegrationTests.Common;

namespace Infrastructure.IntegrationTests.Services;

public sealed class TemplateRenderingTests : PdfIntegrationTestBase
{
    public TemplateRenderingTests(PdfTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public void SimpleTemplate_RendersStringProperty()
    {
        const string template = "Hello {{Name}}!";
        var compiled = Handlebars.Compile(template);
        var model = new SimpleModel("World");

        var result = compiled(model);

        result.Should().Be("Hello World!");
    }

    [Fact]
    public void NestedObject_RendersPropertyPath()
    {
        const string template = "City: {{Address.City}}";
        var compiled = Handlebars.Compile(template);
        var model = new NestedModel("Test", new AddressModel("New York", "Main St"));

        var result = compiled(model);

        result.Should().Be("City: New York");
    }

    [Fact]
    public void Collection_IteratesAllItems()
    {
        const string template = "{{#Items}}{{Name}},{{/Items}}";
        var compiled = Handlebars.Compile(template);
        var model = new CollectionModel("Items",
        [
            new ItemModel("A", 10),
            new ItemModel("B", 20),
            new ItemModel("C", 30),
        ]);

        var result = compiled(model);

        result.Should().Be("A,B,C,");
    }

    [Fact]
    public void Collection_EmptyCollection_RendersNothing()
    {
        const string template = "{{#Items}}{{Name}},{{/Items}}";
        var compiled = Handlebars.Compile(template);
        var model = new CollectionModel("Items", []);

        var result = compiled(model);

        result.Should().BeEmpty();
    }

    [Fact]
    public void IfConditional_TrueBranch_Renders()
    {
        const string template = "{{#if ShowDetails}}{{Message}}{{/if}}";
        var compiled = Handlebars.Compile(template);
        var model = new ConditionalModel(true, "Visible");

        var result = compiled(model);

        result.Should().Be("Visible");
    }

    [Fact]
    public void IfConditional_FalseBranch_Skips()
    {
        const string template = "{{#if ShowDetails}}{{Message}}{{/if}}";
        var compiled = Handlebars.Compile(template);
        var model = new ConditionalModel(false, "Hidden");

        var result = compiled(model);

        result.Should().BeEmpty();
    }

    [Fact]
    public void UnlessConditional_RendersWhenFalse()
    {
        const string template = "{{#unless ShowDetails}}Hidden{{/unless}}";
        var compiled = Handlebars.Compile(template);
        var model = new ConditionalModel(false, "");

        var result = compiled(model);

        result.Should().Be("Hidden");
    }

    [Fact]
    public void EachElse_EmptyCollection_RendersElse()
    {
        const string template = "{{#Items}}{{Name}}{{else}}No items{{/Items}}";
        var compiled = Handlebars.Compile(template);
        var model = new CollectionModel("Items", []);

        var result = compiled(model);

        result.Should().Be("No items");
    }

    [Fact]
    public void MissingValue_RendersEmptyString()
    {
        const string template = "{{Missing}}";
        var compiled = Handlebars.Compile(template);
        var model = new SimpleModel("Test");

        var result = compiled(model);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SalesOrderTemplate_RendersOrderId()
    {
        var model = new SalesOrderPdfModel(
            1001, "Customer", "2026-07-30", "Pending",
            [new SalesOrderItemPdfModel("Item", 1, 10, 10)], 10);

        var pdf = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);

        pdf.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SalesOrderTemplate_RendersAllItems()
    {
        var model = new SalesOrderPdfModel(
            1002, "Customer", "2026-07-30", "Pending",
            [
                new SalesOrderItemPdfModel("Product A", 2, 15.00m, 30.00m),
                new SalesOrderItemPdfModel("Product B", 1, 25.00m, 25.00m),
            ], 55.00m);

        var pdf = await PdfGenerator.GenerateAsync(PdfTemplate.SalesOrder, model);

        pdf.Should().NotBeEmpty();
    }
}
