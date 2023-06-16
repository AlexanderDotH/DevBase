namespace DevBase.Avalonia.Color.Extensions;

public static class ColorNormalizerExtension
{
    public static double[] Normalize(this global::Avalonia.Media.Color color)
    {
        double[] array = new double[4];
        array[0] = Math.Clamp(color.A / 255.0, 0.0, 1.0);
        array[1] = Math.Clamp(color.R / 255.0, 0.0, 1.0);
        array[2] = Math.Clamp(color.G / 255.0, 0.0, 1.0);
        array[3] = Math.Clamp(color.B / 255.0, 0.0, 1.0);
        
        return array;
    }
    
    public static global::Avalonia.Media.Color DeNormalize(this double[] normalized)
    {
        double r = Math.Clamp(normalized[0] * 255.0, 0.0, 255.0);
        double g = Math.Clamp(normalized[1] * 255.0, 0.0, 255.0);
        double b = Math.Clamp(normalized[2] * 255.0, 0.0, 255.0);
        
        return new global::Avalonia.Media.Color(255, (byte)r, (byte)g, (byte)b);
    }
}