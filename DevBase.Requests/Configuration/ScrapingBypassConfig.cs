namespace DevBase.Requests.Configuration;

public enum RefererStrategy
{
    None,
    PreviousUrl,
    BaseHost,
    SearchEngine
}

public enum BrowserProfile
{
    None,
    Chrome,
    Firefox,
    Edge,
    Safari
}

public sealed class ScrapingBypassConfig
{
    public bool Enabled { get; init; }
    public RefererStrategy RefererStrategy { get; init; } = RefererStrategy.None;
    public BrowserProfile BrowserProfile { get; init; } = BrowserProfile.None;
    public bool RandomizeUserAgent { get; init; } = true;
    public bool PersistCookies { get; init; } = true;
    public bool EnableTlsSpoofing { get; init; }

    public static ScrapingBypassConfig Default => new()
    {
        Enabled = true,
        RefererStrategy = RefererStrategy.PreviousUrl,
        BrowserProfile = BrowserProfile.Chrome,
        RandomizeUserAgent = true,
        PersistCookies = true
    };
}
