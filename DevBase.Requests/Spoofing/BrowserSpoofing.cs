using DevBase.Requests.Configuration;

namespace DevBase.Requests.Spoofing;

public static class BrowserSpoofing
{
    private static readonly string[] SearchEngines =
    [
        "https://www.google.com/",
        "https://www.bing.com/",
        "https://duckduckgo.com/",
        "https://www.yahoo.com/",
        "https://www.ecosia.org/"
    ];

    public static void ApplyBrowserProfile(Request request, BrowserProfile profile)
    {
        var headers = GetBrowserHeaders(profile);
        foreach (var header in headers)
        {
            request.WithHeader(header.Key, header.Value);
        }
    }

    public static Dictionary<string, string> GetBrowserHeaders(BrowserProfile profile)
    {
        return profile switch
        {
            BrowserProfile.Chrome => GetChromeHeaders(),
            BrowserProfile.Firefox => GetFirefoxHeaders(),
            BrowserProfile.Edge => GetEdgeHeaders(),
            BrowserProfile.Safari => GetSafariHeaders(),
            _ => new Dictionary<string, string>()
        };
    }

    private static Dictionary<string, string> GetChromeHeaders()
    {
        var version = Random.Shared.Next(120, 131);
        return new Dictionary<string, string>
        {
            ["sec-ch-ua"] = $"\"Chromium\";v=\"{version}\", \"Google Chrome\";v=\"{version}\", \"Not-A.Brand\";v=\"99\"",
            ["sec-ch-ua-mobile"] = "?0",
            ["sec-ch-ua-platform"] = "\"Windows\"",
            ["Upgrade-Insecure-Requests"] = "1",
            ["User-Agent"] = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/537.36",
            ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8",
            ["Accept-Language"] = "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7",
            ["Accept-Encoding"] = "gzip, deflate, br",
            ["sec-fetch-site"] = "none",
            ["sec-fetch-mode"] = "navigate",
            ["sec-fetch-user"] = "?1",
            ["sec-fetch-dest"] = "document"
        };
    }

    private static Dictionary<string, string> GetFirefoxHeaders()
    {
        var version = Random.Shared.Next(120, 134);
        return new Dictionary<string, string>
        {
            ["User-Agent"] = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:{version}.0) Gecko/20100101 Firefox/{version}.0",
            ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8",
            ["Accept-Language"] = "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7",
            ["Accept-Encoding"] = "gzip, deflate, br",
            ["DNT"] = "1",
            ["Upgrade-Insecure-Requests"] = "1",
            ["Sec-Fetch-Dest"] = "document",
            ["Sec-Fetch-Mode"] = "navigate",
            ["Sec-Fetch-Site"] = "none",
            ["Sec-Fetch-User"] = "?1"
        };
    }

    private static Dictionary<string, string> GetEdgeHeaders()
    {
        var version = Random.Shared.Next(120, 131);
        return new Dictionary<string, string>
        {
            ["sec-ch-ua"] = $"\"Microsoft Edge\";v=\"{version}\", \"Chromium\";v=\"{version}\", \"Not-A.Brand\";v=\"99\"",
            ["sec-ch-ua-mobile"] = "?0",
            ["sec-ch-ua-platform"] = "\"Windows\"",
            ["Upgrade-Insecure-Requests"] = "1",
            ["User-Agent"] = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/537.36 Edg/{version}.0.0.0",
            ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
            ["Accept-Language"] = "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7",
            ["Accept-Encoding"] = "gzip, deflate, br"
        };
    }

    private static Dictionary<string, string> GetSafariHeaders()
    {
        var version = Random.Shared.Next(16, 18);
        return new Dictionary<string, string>
        {
            ["User-Agent"] = $"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/{version}.0 Safari/605.1.15",
            ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
            ["Accept-Language"] = "de-DE,de;q=0.9",
            ["Accept-Encoding"] = "gzip, deflate, br"
        };
    }

    public static string GetRandomSearchEngineReferer()
    {
        return SearchEngines[Random.Shared.Next(SearchEngines.Length)];
    }

    public static void ApplyRefererStrategy(Request request, RefererStrategy strategy, string? previousUrl = null)
    {
        var referer = strategy switch
        {
            RefererStrategy.PreviousUrl when previousUrl != null => previousUrl,
            RefererStrategy.BaseHost when request.GetUri() != null => $"{request.GetUri()!.Scheme}://{request.GetUri()!.Host}/",
            RefererStrategy.SearchEngine => GetRandomSearchEngineReferer(),
            _ => null
        };

        if (referer != null)
            request.WithReferer(referer);
    }
}
