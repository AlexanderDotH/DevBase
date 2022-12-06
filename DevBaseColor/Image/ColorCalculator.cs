using Avalonia.Input;
using Avalonia.Media.Imaging;
using DevBase.Generic;

namespace DevBaseColor.Image;

public class ColorCalculator
{
    private Avalonia.Media.Color _brightestColor;

    public ColorCalculator()
    {
        this._brightestColor = new Avalonia.Media.Color();
    }
    
    public unsafe Avalonia.Media.Color GetColorFromBitmap(IBitmap bitmap)
    {
        GenericList<Avalonia.Media.Color> pixels = GetPixels(bitmap);
        GenericList<Avalonia.Media.Color> additional = new GenericList<Avalonia.Media.Color>();

        double colorRange = 50;

        for (int i = 0; i < pixels.Length; i++)
        {
            Avalonia.Media.Color p = pixels.Get(i);

            if (IsInRange(p.R - colorRange, p.R + colorRange, this._brightestColor.R) &&
                IsInRange(p.G - colorRange, p.G + colorRange, this._brightestColor.G) &&
                IsInRange(p.B - colorRange, p.B + colorRange, this._brightestColor.B))
            {
                additional.Add(p);
            }
        }

        double r = 0;
        double g = 0;
        double b = 0;

        double bigShift = 1.5;
        double smallShift = 0.5;
        
        for (int i = 0; i < additional.Length; i++)
        {
            Avalonia.Media.Color pixel = additional.Get(i);

            double red = pixel.R;
            double green = pixel.G;
            double blue = pixel.B;

            if (red > Math.Max(green, blue))
            {
                r += red * bigShift;
            }
            else
            {
                r += red * smallShift;
            }
            
            if (green > Math.Max(red, blue))
            {
                g += green * bigShift;
            }
            else
            {
                g += green * smallShift;
            }
            
            if (blue > Math.Max(red, green))
            {
                b += blue * bigShift;
            }
            else
            {
                b += blue * smallShift;
            }
            
            /*r += pixel.R * 0.8;
            g += pixel.G * 0.8;
            b += pixel.B * 0.8;*/
        }

        r /= additional.Length;
        g /= additional.Length;
        b /= additional.Length;

        return CorrectColor(r, g, b);
    }

    private Avalonia.Media.Color CorrectColor(double r, double g, double b)
    {
        if (Double.IsNaN(r))
            r = 0;

        if (Double.IsNaN(g))
            g = 0;

        if (Double.IsNaN(b))
            b = 0;
        
        if (r > 255)
            r = 255;

        if (g > 255)
            g = 255;

        if (b > 255)
            b = 255;

        byte rB = Convert.ToByte(r);
        byte gB = Convert.ToByte(g);
        byte bB = Convert.ToByte(b);

        return new Avalonia.Media.Color(255, rB, gB, bB);
    }

   
    private bool IsInRange(double min, double max, double current)
    {
        return min < current && max > current;
    }

    private unsafe GenericList<Avalonia.Media.Color> GetPixels(IBitmap bitmap)
    {
        GenericList<Avalonia.Media.Color> colors = new GenericList<Avalonia.Media.Color>();
        
        double brightness = 0;
       
        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var writeableBitmap = WriteableBitmap.Decode(memoryStream);
            using var lockedBitmap = writeableBitmap.Lock();

            byte* bmpPtr = (byte*) lockedBitmap.Address;
            int width = writeableBitmap.PixelSize.Width;
            int height = writeableBitmap.PixelSize.Height;
            byte* tempPtr;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    byte red = *bmpPtr++;
                    byte green = *bmpPtr++;
                    byte blue = *bmpPtr++;
                    byte alpha = *bmpPtr++;
                    
                    double b = (red / 255.0) * 0.3 + (green / 255.0) * 0.59 + (blue / 255.0) * 0.11;

                    if (b > brightness)
                    {
                        brightness = b;
                        this._brightestColor = new Avalonia.Media.Color(alpha, red, green, blue);
                    }

                    if (row % 10 == 0 && col % 10 == 0)
                    {
                        colors.Add(new Avalonia.Media.Color(alpha, red, green, blue));
                    }
                        
                    tempPtr = bmpPtr;
                    bmpPtr = tempPtr;
                }
            }
            
            memoryStream.Close();
        }

        return colors;
    }
}