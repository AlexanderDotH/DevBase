using DevBase.Net.Configuration.Enums;

namespace DevBase.Net.Configuration;

/// <summary>
/// Configuration to bypass anti-scraping measures.
/// </summary>
public sealed class ScrapingBypassConfig
{
    /// <summary>
    /// Gets the strategy for handling the Referer header. Defaults to None.
    /// </summary>
    public EnumRefererStrategy RefererStrategy { get; init; } = EnumRefererStrategy.None;
    
    /// <summary>
    /// Gets the browser profile to emulate. Defaults to None.
    /// </summary>
    public EnumBrowserProfile BrowserProfile { get; init; } = EnumBrowserProfile.None;

    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static ScrapingBypassConfig Default => new()
    {
        RefererStrategy = EnumRefererStrategy.PreviousUrl,
        BrowserProfile = EnumBrowserProfile.Chrome
    };
}
