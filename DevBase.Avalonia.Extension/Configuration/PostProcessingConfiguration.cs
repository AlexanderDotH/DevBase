namespace DevBase.Avalonia.Extension.Configuration;

/// <summary>
/// Configuration for post-processing of calculated colors.
/// </summary>
public class PostProcessingConfiguration
{
    /// <summary>
    /// Gets or sets the small shift value for color shifting.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value for color shifting.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether color shifting post-processing is enabled.
    /// </summary>
    public bool ColorShiftingPostProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets the target lightness for pastel processing.
    /// </summary>
    public double PastelLightness { get; set; }
    
    /// <summary>
    /// Gets or sets the lightness subtractor value for pastel processing when lightness is above guidance.
    /// </summary>
    public double PastelLightnessSubtractor { get; set; }
    
    /// <summary>
    /// Gets or sets the saturation multiplier for pastel processing.
    /// </summary>
    public double PastelSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets the lightness threshold to decide how to adjust pastel lightness.
    /// </summary>
    public double PastelGuidance { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether pastel post-processing is enabled.
    /// </summary>
    public bool PastelPostProcessing { get; set; }
}