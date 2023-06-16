﻿using Colourful;
using DevBase.Avalonia.Color.Converter;
using DevBase.Extensions;
using DevBase.Generics;
using Microsoft.Win32.SafeHandles;

namespace DevBase.Avalonia.Color.Extensions;

public static class LabColorExtension
{
    public static AList<LabColor> FilterBrightness(this AList<LabColor> colors, double percentage)
    {
        AList<LabColor> c = new AList<LabColor>();

        LabColor[] a = new LabColor[colors.Length];

        int count = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            LabColor current = colors.Get(i);

            double brightness = current.L;
            
            if (brightness > percentage)
            {
                a[count] = current;
                count++;
            }
        }
        
        c.AddRange(a.RemoveNullValues());

        return c;
    }
    
    public static AList<LabColor> FilterChroma(this AList<LabColor> colors, double percentage)
    {
        AList<LabColor> c = new AList<LabColor>();

        LabColor[] a = new LabColor[colors.Length];

        int count = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            LabColor current = colors.Get(i);

            double brightness = current.ChromaPercentage();
            
            if (brightness > percentage)
            {
                a[count] = current;
                count++;
            }
        }
        
        c.AddRange(a.RemoveNullValues());

        return c;
    }

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
    
    public static RGBColor ToRgbColor(this double[] normalized)
    {
        return new RGBColor(normalized[1], normalized[2], normalized[3]);
    }

    public static AList<RGBColor> ToRgbColor(this AList<global::Avalonia.Media.Color> color)
    {
        RGBColor[] colors = new RGBColor[color.Length];

        for (int i = 0; i < color.Length; i++)
        {
            colors[i] = color.Get(i).Normalize().ToRgbColor();
        }

        return colors.ToAList();
    }

    public static LabColor ToLabColor(this RGBColor color, RGBToLabConverter converter)
    {
        return converter.ToLabColor(color);
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
}