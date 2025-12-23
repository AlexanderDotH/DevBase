namespace DevBase.Net.Data.Header.UserAgent.Bogus.Generator;

/// <summary>
/// Metadata about the generated user agent
/// </summary>
public readonly struct UserAgentMetadata
{
    public string UserAgent { get; init; }
    public string BrowserVersion { get; init; }
    public string Platform { get; init; }
    public bool IsMobile { get; init; }
    
    /// <summary>
    /// For Chromium-based browsers (Chrome, Edge), this is the Chromium version
    /// </summary>
    public string? ChromiumVersion { get; init; }
}

public interface IBogusUserAgentGenerator
{
    public ReadOnlySpan<char> UserAgentPart { get; }
    
    /// <summary>
    /// Returns the user agent string along with metadata about the generated agent.
    /// This avoids regex extraction overhead.
    /// </summary>
    UserAgentMetadata Generate();
}