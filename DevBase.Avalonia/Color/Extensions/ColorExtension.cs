using DevBase.Generics;

namespace DevBase.Avalonia.Color.Extensions;

/// <summary>
/// Provides extension methods for <see cref="global::Avalonia.Media.Color"/>.
/// </summary>
public static class ColorExtension
{
    /// <summary>
    /// Shifts the RGB components of the color based on their relative intensity.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    /// <param name="bigShift">The multiplier for the dominant color component.</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/> with shifted values.</returns>
    public static global::Avalonia.Media.Color Shift(
        this global::Avalonia.Media.Color color, 
        double smallShift,
        double bigShift)
    {
        double red = color.R;
        double green = color.G;
        double blue = color.B;

        red *= red > Math.Max(green, blue) ? bigShift : smallShift;
        green *= green > Math.Max(red, blue) ? bigShift : smallShift;
        blue *= blue > Math.Max(red, green) ? bigShift : smallShift;

        return new global::Avalonia.Media.Color(color.A, (byte)red, (byte)green, (byte)blue).Correct();
    }

    /// <summary>
    /// Adjusts the brightness of the color by a percentage.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="percentage">The percentage to adjust brightness (e.g., 50 for 50%).</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/> with adjusted brightness.</returns>
    public static global::Avalonia.Media.Color AdjustBrightness(
        this global::Avalonia.Media.Color color,
        double percentage)
    {
        byte r = (byte)((color.R * 0.01) * percentage);
        byte g = (byte)((color.G * 0.01) * percentage);
        byte b = (byte)((color.B * 0.01) * percentage);

        return new global::Avalonia.Media.Color(color.A, r, g, b).Correct();
    }
    
    /// <summary>
    /// Calculates the saturation of the color (0.0 to 1.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The saturation value.</returns>
    public static double Saturation(this global::Avalonia.Media.Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;
        
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        double saturation = 0;
        
        if (max != 0)
        {
            saturation = 1 - (min / max);
        }

        return saturation;
    }
    
    /// <summary>
    /// Calculates the saturation percentage of the color (0.0 to 100.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The saturation percentage.</returns>
    public static double SaturationPercentage(this global::Avalonia.Media.Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;
        
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        double saturation = 0;
        
        if (max != 0)
        {
            saturation = 1 - (min / max);
        }

        return saturation * 100;
    }
    
    /// <summary>
    /// Calculates the brightness of the color using weighted RGB values.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The brightness value.</returns>
    public static double Brightness(this global::Avalonia.Media.Color color)
    {
        return Math.Sqrt(
            0.299 * color.R * color.R + 
            0.587 * color.G * color.G + 
            0.114 * color.B * color.B);
    }
    
    /// <summary>
    /// Calculates the brightness percentage of the color (0.0 to 100.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The brightness percentage.</returns>
    public static double BrightnessPercentage(this global::Avalonia.Media.Color color)
    {
        return Math.Sqrt(
            0.299 * color.R * color.R + 
            0.587 * color.G * color.G + 
            0.114 * color.B * color.B) / 255.0 * 100.0;
    }

    /// <summary>
    /// Calculates the similarity between two colors as a percentage.
    /// </summary>
    /// <param name="color">The first color.</param>
    /// <param name="otherColor">The second color.</param>
    /// <returns>The similarity percentage (0.0 to 100.0).</returns>
    public static double Similarity(this global::Avalonia.Media.Color color, global::Avalonia.Media.Color otherColor)
    {
        int redDifference = color.R - otherColor.R;
        int greenDifference = color.G - otherColor.G;
        int blueDifference = color.B - otherColor.B;

        double distance = Math.Sqrt(redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference);
        double maxDistance = Math.Sqrt(3 * 255 * 255);

        double similarity = 1 - (distance / maxDistance);

        return similarity * 100;
    }
    
    /// <summary>
    /// Corrects the color component values to ensure they are within the valid range (0-255).
    /// </summary>
    /// <param name="color">The color to correct.</param>
    /// <returns>A corrected <see cref="global::Avalonia.Media.Color"/>.</returns>
    public static global::Avalonia.Media.Color Correct(this global::Avalonia.Media.Color color)
    {
        double r = color.R;
        double g = color.G;
        double b = color.B;
        
        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);

        byte rB = Convert.ToByte(r);
        byte gB = Convert.ToByte(g);
        byte bB = Convert.ToByte(b);

        return new global::Avalonia.Media.Color(255, rB, gB, bB);
    }
    
    /// <summary>
    /// Calculates the average color from a list of colors.
    /// </summary>
    /// <param name="colors">The list of colors.</param>
    /// <returns>The average color.</returns>
    public static global::Avalonia.Media.Color Average(this AList<global::Avalonia.Media.Color> colors)
    {
        long sumR = 0;
        long sumG = 0;
        long sumB = 0;

        for (int i = 0; i < colors.Length; i++)
        {
            global::Avalonia.Media.Color c = colors.Get(i);
            
            sumR += c.R;
            sumG += c.G;
            sumB += c.B;
        }

        byte avgR = (byte)(sumR / colors.Length);
        byte avgG = (byte)(sumG / colors.Length);
        byte avgB = (byte)(sumB / colors.Length);

        return new global::Avalonia.Media.Color(255, avgR, avgG, avgB).Correct();
    }
    
    /// <summary>
    /// Filters a list of colors, returning only those with saturation greater than the specified value.
    /// </summary>
    /// <param name="colors">The source list of colors.</param>
    /// <param name="value">The minimum saturation percentage threshold.</param>
    /// <returns>A filtered list of colors.</returns>
    public static AList<global::Avalonia.Media.Color> FilterSaturation(this AList<global::Avalonia.Media.Color> colors, double value)
    {
        AList<global::Avalonia.Media.Color> c = new AList<global::Avalonia.Media.Color>();

        global::Avalonia.Media.Color[] a = new global::Avalonia.Media.Color[colors.Length];

        int count = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            global::Avalonia.Media.Color current = colors.Get(i);

            double saturation = current.SaturationPercentage();
            if (saturation > value)
            {
                a[count] = current;
                count++;
            }
        }
        
        c.AddRange(a.RemoveNullValues());

        return c;
    }
    
    /// <summary>
    /// Filters a list of colors, returning only those with brightness greater than the specified percentage.
    /// </summary>
    /// <param name="colors">The source list of colors.</param>
    /// <param name="percentage">The minimum brightness percentage threshold.</param>
    /// <returns>A filtered list of colors.</returns>
    public static AList<global::Avalonia.Media.Color> FilterBrightness(this AList<global::Avalonia.Media.Color> colors, double percentage)
    {
        AList<global::Avalonia.Media.Color> c = new AList<global::Avalonia.Media.Color>();

        global::Avalonia.Media.Color[] a = new global::Avalonia.Media.Color[colors.Length];

        int count = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            global::Avalonia.Media.Color current = colors.Get(i);

            double brightness = current.BrightnessPercentage();
            
            if (brightness > percentage)
            {
                a[count] = current;
                count++;
            }
        }
        
        c.AddRange(a.RemoveNullValues());

        return c;
    }
    
    /// <summary>
    /// Removes transparent colors (alpha=0, rgb=0) from the array.
    /// </summary>
    /// <param name="colors">The source array of colors.</param>
    /// <returns>A new array with null/empty values removed.</returns>
    public static global::Avalonia.Media.Color[] RemoveNullValues(this global::Avalonia.Media.Color[] colors)
    {
        int cap = 0;
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i] != global::Avalonia.Media.Color.FromArgb(0,0,0,0))
                cap++;
        }

        global::Avalonia.Media.Color[] c = new global::Avalonia.Media.Color[cap];

        int counter = 0;
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i] != global::Avalonia.Media.Color.FromArgb(0,0,0,0))
            {
                c[counter] = colors[i];
                counter++;
            }
        }

        return c;
    }

   
}