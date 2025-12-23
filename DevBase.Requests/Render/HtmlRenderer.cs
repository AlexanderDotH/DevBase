using AngleSharp;
using AngleSharp.Dom;
using IConfiguration = AngleSharp.IConfiguration;

namespace DevBase.Requests.Render;

public sealed class HtmlRenderer
{
    private readonly IConfiguration _config;
    private readonly IBrowsingContext _context;

    public HtmlRenderer()
    {
        _config = AngleSharp.Configuration.Default;
        _context = BrowsingContext.New(_config);
    }

    public async Task<IDocument> RenderAsync(string html, CancellationToken cancellationToken = default)
    {
        return await _context.OpenAsync(req => req.Content(html), cancellationToken);
    }

    public async Task<IDocument> RenderAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        string html = await reader.ReadToEndAsync(cancellationToken);
        return await RenderAsync(html, cancellationToken);
    }

    public async Task<IDocument> RenderFromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _context.OpenAsync(url, cancellationToken);
    }

    public static async Task<IDocument> ParseAsync(string html, CancellationToken cancellationToken = default)
    {
        IConfiguration config = AngleSharp.Configuration.Default;
        IBrowsingContext context = BrowsingContext.New(config);
        return await context.OpenAsync(req => req.Content(html), cancellationToken);
    }
}
