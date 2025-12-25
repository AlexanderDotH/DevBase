namespace DevBase.Avalonia.Extension.Configuration;

/// <summary>
/// Configuration for chroma (color intensity) filtering.
/// </summary>
public class ChromaConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether chroma filtering is enabled.
    /// </summary>
    public bool FilterChroma { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum chroma threshold.
    /// </summary>
    public double MinChroma { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum chroma threshold.
    /// </summary>
    public double MaxChroma { get; set; }
}