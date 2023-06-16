using Colourful;

namespace DevBase.Avalonia.Extension.Extension;

public static class ColorNormalizerExtension
{
    public static global::Avalonia.Media.Color DeNormalize(this RGBColor normalized)
    {
        double r = Math.Clamp(normalized.R * 255.0, 0.0, 255.0);
        double g = Math.Clamp(normalized.G * 255.0, 0.0, 255.0);
        double b = Math.Clamp(normalized.B * 255.0, 0.0, 255.0);
        
        return new global::Avalonia.Media.Color(255, (byte)r, (byte)g, (byte)b);
    }
}