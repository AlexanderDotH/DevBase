using Colourful;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Extension.Converter;
using DevBase.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Extension.Extension;

/// <summary>
/// Provides extension methods for LabColor operations.
/// </summary>
public static class LabColorExtension
{
    #region Brightness

    /// <summary>
    /// Filters a list of LabColors based on lightness (L) values.
    /// </summary>
    /// <param name="colors">The list of LabColors.</param>
    /// <param name="min">Minimum lightness.</param>
    /// <param name="max">Maximum lightness.</param>
    /// <returns>A filtered list of LabColors.</returns>
    public static AList<LabColor> FilterBrightness(this AList<LabColor> colors, double min, double max)
    {
        AList<LabColor> c = new AList<LabColor>();

        LabColor[] a = new LabColor[colors.Length];

        int count = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            LabColor current = colors.Get(i);

            double brightness = current.L;
            
            if (brightness >= min && brightness <= max)
            {
                a[count] = current;
                count++;
            }
        }
        
        LabColor[] color = a.RemoveNullValues();

        if (colors.Length == 0)
            color = color.ToArray();
        
        c.AddRange(color);

        return c;
    }

    #endregion

    #region Chroma

    /// <summary>
    /// Calculates the chroma of a LabColor.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <returns>The chroma value.</returns>
    public static double Chroma(this LabColor color)
    {
        double a = color.a;
        double b = color.b;
        return Math.Sqrt(a * a + b * b);
    }
    
    /// <summary>
    /// Calculates the chroma percentage relative to a max chroma of 128.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <returns>The chroma percentage.</returns>
    public static double ChromaPercentage(this LabColor color)
    {
        return (color.Chroma() / 128) * 100;
    }
    
    /// <summary>
    /// Filters a list of LabColors based on chroma percentage.
    /// </summary>
    /// <param name="colors">The list of LabColors.</param>
    /// <param name="min">Minimum chroma percentage.</param>
    /// <param name="max">Maximum chroma percentage.</param>
    /// <returns>A filtered list of LabColors.</returns>
    public static AList<LabColor> FilterChroma(this AList<LabColor> colors, double min, double max)
    {
        AList<LabColor> c = new AList<LabColor>();

        LabColor[] a = new LabColor[colors.Length];

        int count = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            LabColor current = colors.Get(i);

            double brightness = current.ChromaPercentage();
            
            if (brightness >= min && brightness <= max)
            {
                a[count] = current;
                count++;
            }
        }

        LabColor[] color = a.RemoveNullValues();

        if (colors.Length == 0)
            color = color.ToArray();
        
        c.AddRange(color);

        return c;
    }

    #endregion

    #region Converter

    /// <summary>
    /// Converts a normalized double array to an RGBColor.
    /// </summary>
    /// <param name="normalized">Normalized array [A, R, G, B] or similar.</param>
    /// <returns>The RGBColor.</returns>
    public static RGBColor ToRgbColor(this double[] normalized)
    {
        return new RGBColor(normalized[1], normalized[2], normalized[3]);
    }

    /// <summary>
    /// Converts an RGBColor to LabColor using the provided converter.
    /// </summary>
    /// <param name="color">The RGBColor.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The LabColor.</returns>
    public static LabColor ToLabColor(this RGBColor color, RGBToLabConverter converter) => 
        converter.ToLabColor(color);

    /// <summary>
    /// Converts a LabColor to RGBColor using the provided converter.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The RGBColor.</returns>
    public static RGBColor ToRgbColor(this LabColor color, RGBToLabConverter converter) => 
        converter.ToRgbColor(color);

    #endregion

    #region Processing

    /// <summary>
    /// Adjusts a LabColor to be more pastel-like by modifying lightness and saturation.
    /// </summary>
    /// <param name="color">The original LabColor.</param>
    /// <param name="lightness">The lightness to add.</param>
    /// <param name="saturation">The saturation multiplier.</param>
    /// <returns>The pastel LabColor.</returns>
    public static LabColor ToPastel(this LabColor color, double lightness = 20.0d, double saturation = 0.5d)
    {
        double l = Math.Min(100.0d, color.L + lightness);
        double a = color.a * saturation;
        double b = color.b * saturation;
        return new LabColor(l, a, b);
    }

    #endregion
    
    #region Bulk Converter

    /// <summary>
    /// Converts a list of Avalonia Colors to RGBColors.
    /// </summary>
    /// <param name="color">The list of Avalonia Colors.</param>
    /// <returns>A list of RGBColors.</returns>
    public static AList<RGBColor> ToRgbColor(this AList<global::Avalonia.Media.Color> color)
    {
        RGBColor[] colors = new RGBColor[color.Length];

        for (int i = 0; i < color.Length; i++)
        {
            colors[i] = color.Get(i).Normalize().ToRgbColor();
        }

        return colors.ToAList();
    }

    /// <summary>
    /// Converts a list of RGBColors to LabColors using the provided converter.
    /// </summary>
    /// <param name="colors">The list of RGBColors.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>A list of LabColors.</returns>
    public static AList<LabColor> ToLabColor(this AList<RGBColor> colors, RGBToLabConverter converter)
    {
        LabColor[] outColors = new LabColor[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            outColors[i] = colors.Get(i).ToLabColor(converter);
        }

        return outColors.ToAList();
    }

    #endregion

    #region Correction

    /// <summary>
    /// Removes default LabColor (0,0,0) values from an array.
    /// </summary>
    /// <param name="colors">The source array.</param>
    /// <returns>An array with default values removed.</returns>
    public static LabColor[] RemoveNullValues(this LabColor[] colors)
    {
        int cap = 0;
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i] != new LabColor(0,0,0))
                cap++;
        }

        LabColor[] c = new LabColor[cap];

        int counter = 0;
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i]  != new LabColor(0,0,0))
            {
                c[counter] = colors[i];
                counter++;
            }
        }

        return c;
    }

    #endregion
}