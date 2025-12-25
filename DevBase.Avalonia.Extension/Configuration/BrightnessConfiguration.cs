namespace DevBase.Avalonia.Extension.Configuration;

/// <summary>
/// Configuration for brightness filtering.
/// </summary>
public class BrightnessConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether brightness filtering is enabled.
    /// </summary>
    public bool FilterBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold (0-100).
    /// </summary>
    public double MinBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum brightness threshold (0-100).
    /// </summary>
    public double MaxBrightness { get; set; }
}