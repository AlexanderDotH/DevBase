using Colourful;

namespace DevBase.Avalonia.Extension.Extension;

/// <summary>
/// Provides extension methods for color normalization.
/// </summary>
public static class ColorNormalizerExtension
{
    /// <summary>
    /// Denormalizes an RGBColor (0-1 range) to an Avalonia Color (0-255 range).
    /// </summary>
    /// <param name="normalized">The normalized RGBColor.</param>
    /// <returns>The denormalized Avalonia Color.</returns>
    public static global::Avalonia.Media.Color DeNormalize(this RGBColor normalized)
    {
        double r = Math.Clamp(normalized.R * 255.0, 0.0, 255.0);
        double g = Math.Clamp(normalized.G * 255.0, 0.0, 255.0);
        double b = Math.Clamp(normalized.B * 255.0, 0.0, 255.0);
        
        return new global::Avalonia.Media.Color(255, (byte)r, (byte)g, (byte)b);
    }
}