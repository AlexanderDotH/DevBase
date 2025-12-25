namespace DevBase.Avalonia.Color.Extensions;

/// <summary>
/// Provides extension methods for normalizing color values.
/// </summary>
public static class ColorNormalizerExtension
{
    /// <summary>
    /// Normalizes the color components to a range of 0.0 to 1.0.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>An array containing normalized [A, R, G, B] values.</returns>
    public static double[] Normalize(this global::Avalonia.Media.Color color)
    {
        double[] array = new double[4];
        array[0] = Math.Clamp(color.A / 255.0, 0.0, 1.0);
        array[1] = Math.Clamp(color.R / 255.0, 0.0, 1.0);
        array[2] = Math.Clamp(color.G / 255.0, 0.0, 1.0);
        array[3] = Math.Clamp(color.B / 255.0, 0.0, 1.0);
        
        return array;
    }
    
    /// <summary>
    /// Denormalizes an array of [A, R, G, B] (or [R, G, B]) values back to a Color.
    /// </summary>
    /// <param name="normalized">The normalized color array (values 0.0 to 1.0).</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/>.</returns>
    public static global::Avalonia.Media.Color DeNormalize(this double[] normalized)
    {
        double r = Math.Clamp(normalized[0] * 255.0, 0.0, 255.0);
        double g = Math.Clamp(normalized[1] * 255.0, 0.0, 255.0);
        double b = Math.Clamp(normalized[2] * 255.0, 0.0, 255.0);
        
        return new global::Avalonia.Media.Color(255, (byte)r, (byte)g, (byte)b);
    }
}