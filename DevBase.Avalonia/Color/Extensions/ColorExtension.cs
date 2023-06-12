using Avalonia.Media;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Extensions;

public static class ColorExtension
{
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
    
    public static double Brightness(this global::Avalonia.Media.Color color)
    {
        return Math.Sqrt(
            0.299 * color.R * color.R + 
            0.587 * color.G * color.G + 
            0.114 * color.B * color.B);
    }
    
    public static double BrightnessPercentage(this global::Avalonia.Media.Color color)
    {
        return Math.Sqrt(
            0.299 * color.R * color.R + 
            0.587 * color.G * color.G + 
            0.114 * color.B * color.B) / 255.0 * 100.0;
    }

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