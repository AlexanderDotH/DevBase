using Colourful;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Extension.Converter;
using DevBase.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Extension.Extension;

public static class LabColorExtension
{
    #region Brightness

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

    public static double Chroma(this LabColor color)
    {
        double a = color.a;
        double b = color.b;
        return Math.Sqrt(a * a + b * b);
    }
    
    public static double ChromaPercentage(this LabColor color)
    {
        return (color.Chroma() / 128) * 100;
    }
    
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

    public static RGBColor ToRgbColor(this double[] normalized)
    {
        return new RGBColor(normalized[1], normalized[2], normalized[3]);
    }

    public static LabColor ToLabColor(this RGBColor color, RGBToLabConverter converter) => 
        converter.ToLabColor(color);

    public static RGBColor ToRgbColor(this LabColor color, RGBToLabConverter converter) => 
        converter.ToRgbColor(color);

    #endregion

    #region Processing

    public static LabColor ToPastel(this LabColor color, double lightness = 20.0d, double saturation = 0.5d)
    {
        double l = Math.Min(100.0d, color.L + lightness);
        double a = color.a * saturation;
        double b = color.b * saturation;
        return new LabColor(l, a, b);
    }

    #endregion
    
    #region Bulk Converter

    public static AList<RGBColor> ToRgbColor(this AList<global::Avalonia.Media.Color> color)
    {
        RGBColor[] colors = new RGBColor[color.Length];

        for (int i = 0; i < color.Length; i++)
        {
            colors[i] = color.Get(i).Normalize().ToRgbColor();
        }

        return colors.ToAList();
    }

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