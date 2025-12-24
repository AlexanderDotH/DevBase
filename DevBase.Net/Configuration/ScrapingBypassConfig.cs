using DevBase.Net.Configuration.Enums;

namespace DevBase.Net.Configuration;

public sealed class ScrapingBypassConfig
{
    public EnumRefererStrategy RefererStrategy { get; init; } = EnumRefererStrategy.None;
    public EnumBrowserProfile BrowserProfile { get; init; } = EnumBrowserProfile.None;

    public static ScrapingBypassConfig Default => new()
    {
        RefererStrategy = EnumRefererStrategy.PreviousUrl,
        BrowserProfile = EnumBrowserProfile.Chrome
    };
}
