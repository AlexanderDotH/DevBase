using System.Text;
using DevBase.Net.Configuration.Enums;
using DevBase.Net.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Net.Utils;

namespace DevBase.Net.Spoofing;

public static class BrowserSpoofing
{
    
    // Header names (used multiple times)
    private static readonly ReadOnlyMemory<char> HeaderSecChUa = "sec-ch-ua".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderSecChUaMobile = "sec-ch-ua-mobile".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderSecChUaPlatform = "sec-ch-ua-platform".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderUpgradeInsecureRequests = "Upgrade-Insecure-Requests".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderAcceptLanguage = "Accept-Language".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderAcceptEncoding = "Accept-Encoding".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderSecFetchSite = "sec-fetch-site".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderSecFetchMode = "sec-fetch-mode".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderSecFetchUser = "sec-fetch-user".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderSecFetchDest = "sec-fetch-dest".AsMemory();
    private static readonly ReadOnlyMemory<char> HeaderDnt = "DNT".AsMemory();
    
    // Header values (used multiple times)
    private static readonly ReadOnlyMemory<char> ValueOne = "1".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueQuestionOne = "?1".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueQuestionZero = "?0".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueNone = "none".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueNavigate = "navigate".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueDocument = "document".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptEncodingGzip = "gzip, deflate, br".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptLanguageDefault = "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptLanguageSafari = "de-DE,de;q=0.9".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptChromium = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptFirefox = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptEdge = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8".AsMemory();
    private static readonly ReadOnlyMemory<char> ValueAcceptSafari = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8".AsMemory();
    
    // sec-ch-ua building blocks
    private static readonly char[] ChUaChromiumPrefix = "\"Chromium\";v=\"".ToCharArray();
    private static readonly char[] ChUaChromePrefix = "\", \"Google Chrome\";v=\"".ToCharArray();
    private static readonly char[] ChUaEdgePrefix = "\"Microsoft Edge\";v=\"".ToCharArray();
    private static readonly char[] ChUaChromiumInfix = "\", \"Chromium\";v=\"".ToCharArray();
    private static readonly char[] ChUaSuffix = "\", \"Not-A.Brand\";v=\"99\"".ToCharArray();
    
    private static readonly string[] SearchEngines =
    [
        "https://www.google.com/",
        "https://www.bing.com/",
        "https://duckduckgo.com/",
        "https://www.yahoo.com/",
        "https://www.ecosia.org/"
    ];

    public static void ApplyBrowserProfile(Core.Request request, EnumBrowserProfile profile)
    {
        switch (profile)
        {
            case EnumBrowserProfile.Chrome:
                ApplyChromeHeaders(request);
                break;
            case EnumBrowserProfile.Firefox:
                ApplyFirefoxHeaders(request);
                break;
            case EnumBrowserProfile.Edge:
                ApplyEdgeHeaders(request);
                break;
            case EnumBrowserProfile.Safari:
                ApplySafariHeaders(request);
                break;
        }
    }

    private static void ApplyChromeHeaders(Core.Request request)
    {
        BogusChromeUserAgentGenerator generator = new BogusChromeUserAgentGenerator();
        UserAgentMetadata metadata = generator.Generate();
        
        StringBuilder sb = StringBuilderPool.Acquire(128);
        
        // sec-ch-ua header
        sb.Append(ChUaChromiumPrefix);
        sb.Append(metadata.ChromiumVersion);
        sb.Append(ChUaChromePrefix);
        sb.Append(metadata.BrowserVersion);
        sb.Append(ChUaSuffix);
        request.WithHeader(HeaderSecChUa, sb.ToString());
        
        request.WithHeader(HeaderSecChUaMobile, metadata.IsMobile ? ValueQuestionOne : ValueQuestionZero);
        
        // sec-ch-ua-platform header
        sb.Clear();
        sb.Append('"');
        sb.Append(metadata.Platform);
        sb.Append('"');
        request.WithHeader(HeaderSecChUaPlatform, sb.ToString());
        
        request.WithHeader(HeaderUpgradeInsecureRequests, ValueOne);
        request.WithUserAgent(metadata.UserAgent);
        request.WithAccept(ValueAcceptChromium);
        request.WithHeader(HeaderAcceptLanguage, ValueAcceptLanguageDefault);
        request.WithHeader(HeaderAcceptEncoding, ValueAcceptEncodingGzip);
        request.WithHeader(HeaderSecFetchSite, ValueNone);
        request.WithHeader(HeaderSecFetchMode, ValueNavigate);
        request.WithHeader(HeaderSecFetchUser, ValueQuestionOne);
        request.WithHeader(HeaderSecFetchDest, ValueDocument);
    }

    private static void ApplyFirefoxHeaders(Core.Request request)
    {
        BogusFirefoxUserAgentGenerator generator = new BogusFirefoxUserAgentGenerator();
        UserAgentMetadata metadata = generator.Generate();
        
        request.WithUserAgent(metadata.UserAgent);
        request.WithAccept(ValueAcceptFirefox);
        request.WithHeader(HeaderAcceptLanguage, ValueAcceptLanguageDefault);
        request.WithHeader(HeaderAcceptEncoding, ValueAcceptEncodingGzip);
        request.WithHeader(HeaderDnt, ValueOne);
        request.WithHeader(HeaderUpgradeInsecureRequests, ValueOne);
        request.WithHeader(HeaderSecFetchDest, ValueDocument);
        request.WithHeader(HeaderSecFetchMode, ValueNavigate);
        request.WithHeader(HeaderSecFetchSite, ValueNone);
        request.WithHeader(HeaderSecFetchUser, ValueQuestionOne);
    }

    private static void ApplyEdgeHeaders(Core.Request request)
    {
        BogusEdgeUserAgentGenerator generator = new BogusEdgeUserAgentGenerator();
        UserAgentMetadata metadata = generator.Generate();
        
        StringBuilder sb = StringBuilderPool.Acquire(128);
        
        // sec-ch-ua header
        sb.Append(ChUaEdgePrefix);
        sb.Append(metadata.BrowserVersion);
        sb.Append(ChUaChromiumInfix);
        sb.Append(metadata.ChromiumVersion);
        sb.Append(ChUaSuffix);
        request.WithHeader(HeaderSecChUa, sb.ToString());
        
        request.WithHeader(HeaderSecChUaMobile, metadata.IsMobile ? ValueQuestionOne : ValueQuestionZero);
        
        // sec-ch-ua-platform header
        sb.Clear();
        sb.Append('"');
        sb.Append(metadata.Platform);
        sb.Append('"');
        request.WithHeader(HeaderSecChUaPlatform, sb.ToString());
        
        request.WithHeader(HeaderUpgradeInsecureRequests, ValueOne);
        request.WithUserAgent(metadata.UserAgent);
        request.WithAccept(ValueAcceptEdge);
        request.WithHeader(HeaderAcceptLanguage, ValueAcceptLanguageDefault);
        request.WithHeader(HeaderAcceptEncoding, ValueAcceptEncodingGzip);
        request.WithHeader(HeaderSecFetchSite, ValueNone);
        request.WithHeader(HeaderSecFetchMode, ValueNavigate);
        request.WithHeader(HeaderSecFetchUser, ValueQuestionOne);
        request.WithHeader(HeaderSecFetchDest, ValueDocument);
    }

    private static void ApplySafariHeaders(Core.Request request)
    {
        BogusOperaUserAgentGenerator generator = new BogusOperaUserAgentGenerator();
        UserAgentMetadata metadata = generator.Generate();
        
        request.WithUserAgent(metadata.UserAgent);
        request.WithAccept(ValueAcceptSafari);
        request.WithHeader(HeaderAcceptLanguage, "de-DE,de;q=0.9");
        request.WithHeader(HeaderAcceptEncoding, ValueAcceptEncodingGzip);
    }

    public static string GetRandomSearchEngineReferer()
    {
        return SearchEngines[Random.Shared.Next(SearchEngines.Length)];
    }

    public static void ApplyRefererStrategy(Core.Request request, EnumRefererStrategy strategy, string? previousUrl = null)
    {
        string? referer = strategy switch
        {
            EnumRefererStrategy.PreviousUrl when previousUrl != null => previousUrl,
            EnumRefererStrategy.BaseHost when request.GetUri() != null => BuildBaseHostReferer(request.GetUri()!),
            EnumRefererStrategy.SearchEngine => GetRandomSearchEngineReferer(),
            _ => null
        };

        if (referer != null)
            request.WithReferer(referer);
    }
    
    private static string BuildBaseHostReferer(Uri uri)
    {
        StringBuilder sb = StringBuilderPool.Acquire(64);
        sb.Append(uri.Scheme);
        sb.Append("://");
        sb.Append(uri.Host);
        sb.Append('/');
        return sb.ToString();
    }

}
