using DevBase.Requests.Configuration.Enums;

namespace DevBase.Requests.Configuration;

public sealed class ScrapingBypassConfig
{
    public bool Enabled { get; init; }
    public EnumRefererStrategy RefererStrategy { get; init; } = EnumRefererStrategy.None;
    public EnumBrowserProfile BrowserProfile { get; init; } = EnumBrowserProfile.None;
    public bool RandomizeUserAgent { get; init; } = true;
    public bool PersistCookies { get; init; } = true;
    public bool EnableTlsSpoofing { get; init; }

    public static ScrapingBypassConfig Default => new()
    {
        Enabled = true,
        RefererStrategy = EnumRefererStrategy.PreviousUrl,
        BrowserProfile = EnumBrowserProfile.Chrome,
        RandomizeUserAgent = true,
        PersistCookies = true
    };
}
