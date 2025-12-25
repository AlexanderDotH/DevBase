namespace DevBase.Avalonia.Extension.Configuration;

/// <summary>
/// Configuration for image pre-processing.
/// </summary>
public class PreProcessingConfiguration
{
    /// <summary>
    /// Gets or sets the sigma value for blur.
    /// </summary>
    public float BlurSigma { get; set; }
    
    /// <summary>
    /// Gets or sets the number of blur rounds.
    /// </summary>
    public int BlurRounds { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether blur pre-processing is enabled.
    /// </summary>
    public bool BlurPreProcessing { get; set; }
}