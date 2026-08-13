using PuppeteerSharp;

namespace Infrastructure.Services.PdfGeneration;

internal sealed class BrowserManager : IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IBrowser? _browser;
    private bool _disposed;

    public async Task<IPage> CreatePageAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_browser is null)
            {
                await new BrowserFetcher().DownloadAsync();
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox" }
                });
            }
            return await _browser.NewPageAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_browser is not null)
        {
            await _browser.DisposeAsync();
            _browser = null;
        }

        _semaphore.Dispose();
    }
}
