using System.Text.RegularExpressions;
using DevBase.Requests.Configuration;
using DevBase.Requests.Configuration.Enums;
using DevBase.Requests.Core;
using DevBase.Requests.Data.Header.UserAgent.Bogus.Generator;

namespace DevBase.Requests.Spoofing;

public static partial class BrowserSpoofing
{
    private static readonly string[] SearchEngines =
    [
        "https://www.google.com/",
        "https://www.bing.com/",
        "https://duckduckgo.com/",
        "https://www.yahoo.com/",
        "https://www.ecosia.org/"
    ];

    [GeneratedRegex(@"Chrome/([\d]+)", RegexOptions.Compiled)]
    private static partial Regex ChromeVersionRegex();

    [GeneratedRegex(@"Edg/([\d]+)", RegexOptions.Compiled)]
    private static partial Regex EdgeVersionRegex();

    [GeneratedRegex(@"\(([^)]+)\)", RegexOptions.Compiled)]
    private static partial Regex PlatformRegex();

    public static void ApplyBrowserProfile(Request request, EnumBrowserProfile profile)
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

    private static void ApplyChromeHeaders(Request request)
    {
        BogusChromeUserAgentGenerator generator = new BogusChromeUserAgentGenerator();
        string userAgent = generator.UserAgentPart.ToString();
        
        string chromeVersion = ExtractChromeVersion(userAgent);
        string platform = ExtractPlatform(userAgent);
        bool isMobile = userAgent.Contains("Mobile");
        
        request.WithHeader("sec-ch-ua", $"\"Chromium\";v=\"{chromeVersion}\", \"Google Chrome\";v=\"{chromeVersion}\", \"Not-A.Brand\";v=\"99\"");
        request.WithHeader("sec-ch-ua-mobile", isMobile ? "?1" : "?0");
        request.WithHeader("sec-ch-ua-platform", $"\"{platform}\"");
        request.WithHeader("Upgrade-Insecure-Requests", "1");
        request.WithUserAgent(userAgent);
        request.WithAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
        request.WithHeader("Accept-Language", "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7");
        request.WithHeader("Accept-Encoding", "gzip, deflate, br");
        request.WithHeader("sec-fetch-site", "none");
        request.WithHeader("sec-fetch-mode", "navigate");
        request.WithHeader("sec-fetch-user", "?1");
        request.WithHeader("sec-fetch-dest", "document");
    }

    private static void ApplyFirefoxHeaders(Request request)
    {
        BogusFirefoxUserAgentGenerator generator = new BogusFirefoxUserAgentGenerator();
        string userAgent = generator.UserAgentPart.ToString();
        
        request.WithUserAgent(userAgent);
        request.WithAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
        request.WithHeader("Accept-Language", "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7");
        request.WithHeader("Accept-Encoding", "gzip, deflate, br");
        request.WithHeader("DNT", "1");
        request.WithHeader("Upgrade-Insecure-Requests", "1");
        request.WithHeader("Sec-Fetch-Dest", "document");
        request.WithHeader("Sec-Fetch-Mode", "navigate");
        request.WithHeader("Sec-Fetch-Site", "none");
        request.WithHeader("Sec-Fetch-User", "?1");
    }

    private static void ApplyEdgeHeaders(Request request)
    {
        BogusEdgeUserAgentGenerator generator = new BogusEdgeUserAgentGenerator();
        string userAgent = generator.UserAgentPart.ToString();
        
        string chromeVersion = ExtractChromeVersion(userAgent);
        string edgeVersion = ExtractEdgeVersion(userAgent);
        string platform = ExtractPlatform(userAgent);
        bool isMobile = userAgent.Contains("Mobile");
        
        request.WithHeader("sec-ch-ua", $"\"Microsoft Edge\";v=\"{edgeVersion}\", \"Chromium\";v=\"{chromeVersion}\", \"Not-A.Brand\";v=\"99\"");
        request.WithHeader("sec-ch-ua-mobile", isMobile ? "?1" : "?0");
        request.WithHeader("sec-ch-ua-platform", $"\"{platform}\"");
        request.WithHeader("Upgrade-Insecure-Requests", "1");
        request.WithUserAgent(userAgent);
        request.WithAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
        request.WithHeader("Accept-Language", "de-DE,de;q=0.9,en-US;q=0.8,en;q=0.7");
        request.WithHeader("Accept-Encoding", "gzip, deflate, br");
        request.WithHeader("sec-fetch-site", "none");
        request.WithHeader("sec-fetch-mode", "navigate");
        request.WithHeader("sec-fetch-user", "?1");
        request.WithHeader("sec-fetch-dest", "document");
    }

    private static void ApplySafariHeaders(Request request)
    {
        BogusOperaUserAgentGenerator generator = new BogusOperaUserAgentGenerator();
        string userAgent = generator.UserAgentPart.ToString();
        
        request.WithUserAgent(userAgent);
        request.WithAccept("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        request.WithHeader("Accept-Language", "de-DE,de;q=0.9");
        request.WithHeader("Accept-Encoding", "gzip, deflate, br");
    }

    public static string GetRandomSearchEngineReferer()
    {
        return SearchEngines[Random.Shared.Next(SearchEngines.Length)];
    }

    public static void ApplyRefererStrategy(Request request, EnumRefererStrategy strategy, string? previousUrl = null)
    {
        string? referer = strategy switch
        {
            EnumRefererStrategy.PreviousUrl when previousUrl != null => previousUrl,
            EnumRefererStrategy.BaseHost when request.GetUri() != null => $"{request.GetUri()!.Scheme}://{request.GetUri()!.Host}/",
            EnumRefererStrategy.SearchEngine => GetRandomSearchEngineReferer(),
            _ => null
        };

        if (referer != null)
            request.WithReferer(referer);
    }

    private static string ExtractChromeVersion(string userAgent)
    {
        Match match = ChromeVersionRegex().Match(userAgent);
        return match.Success ? match.Groups[1].Value : "131";
    }

    private static string ExtractEdgeVersion(string userAgent)
    {
        Match match = EdgeVersionRegex().Match(userAgent);
        return match.Success ? match.Groups[1].Value : "131";
    }

    private static string ExtractPlatform(string userAgent)
    {
        Match match = PlatformRegex().Match(userAgent);
        if (!match.Success) return "Windows";
        
        string platformInfo = match.Groups[1].Value;
        
        if (platformInfo.Contains("Windows"))
            return "Windows";
        if (platformInfo.Contains("Macintosh") || platformInfo.Contains("Mac OS"))
            return "macOS";
        if (platformInfo.Contains("Linux"))
            return "Linux";
        if (platformInfo.Contains("Android"))
            return "Android";
        if (platformInfo.Contains("iPhone") || platformInfo.Contains("iPad"))
            return "iOS";
            
        return "Windows";
    }
}
