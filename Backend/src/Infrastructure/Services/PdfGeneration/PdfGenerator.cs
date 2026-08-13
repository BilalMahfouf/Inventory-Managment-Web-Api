using System.Collections.Concurrent;
using System.Reflection;
using Application.PdfGeneration;
using Application.PdfGeneration.Contracts;
using HandlebarsDotNet;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Infrastructure.Services.PdfGeneration;

internal sealed class PdfGenerator : IPdfGenerator, IAsyncDisposable
{
    private static readonly ConcurrentDictionary<PdfTemplate, HandlebarsTemplate<object, object>> CompiledTemplates = new();
    private static readonly Dictionary<PdfTemplate, string> TemplatePaths = new()
    {
        [PdfTemplate.SalesOrder] = "Infrastructure.Services.PdfGeneration.Templates.SalesOrder.hbs"
    };

    private readonly BrowserManager _browserManager;

    public PdfGenerator(BrowserManager browserManager)
    {
        _browserManager = browserManager;
    }

    public async Task<byte[]> GenerateAsync<TModel>(
        PdfTemplate template,
        TModel model,
        CancellationToken cancellationToken = default)
    {
        var renderTemplate = CompiledTemplates.GetOrAdd(template, CompileTemplate);
        var html = renderTemplate(model!);

        var page = await _browserManager.CreatePageAsync(cancellationToken);
        try
        {
            await page.SetContentAsync(html);
            return await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true
            });
        }
        finally
        {
            await page.DisposeAsync();
        }
    }

    private static HandlebarsTemplate<object, object> CompileTemplate(PdfTemplate template)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var path = TemplatePaths[template];
        using var stream = assembly.GetManifestResourceStream(path)
            ?? throw new InvalidOperationException($"Template resource not found: {path}");
        using var reader = new StreamReader(stream);
        var source = reader.ReadToEnd();
        return Handlebars.Compile(source);
    }

    public async ValueTask DisposeAsync()
    {
        await _browserManager.DisposeAsync();
    }
}
