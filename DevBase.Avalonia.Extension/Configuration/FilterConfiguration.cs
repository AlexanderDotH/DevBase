namespace DevBase.Avalonia.Extension.Configuration;

/// <summary>
/// Configuration for color filtering settings.
/// </summary>
public class FilterConfiguration
{
    /// <summary>
    /// Gets or sets the chroma configuration.
    /// </summary>
    public ChromaConfiguration ChromaConfiguration { get; set; }
    
    /// <summary>
    /// Gets or sets the brightness configuration.
    /// </summary>
    public BrightnessConfiguration BrightnessConfiguration { get; set; }
    
}