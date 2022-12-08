using Avalonia.Input;
using Avalonia.Media.Imaging;
using DevBase.Generic;

namespace DevBaseColor.Image;

public class BrightestColorCalculator
{
    private Avalonia.Media.Color _brightestColor;
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    public BrightestColorCalculator()
    {
        this._brightestColor = new Avalonia.Media.Color();

        this._colorRange = 50;
        this._bigShift = 1.5;
        this._smallShift = 0.5;
        this._pixelSteps = 10;
    }

    public BrightestColorCalculator(double bigShift, double smallShift) : this()
    {
        this._bigShift = bigShift;
        this._smallShift = smallShift;
    }
    
    public unsafe Avalonia.Media.Color GetColorFromBitmap(IBitmap bitmap)
    {
        GenericList<Avalonia.Media.Color> pixels = GetPixels(bitmap);
        GenericList<Avalonia.Media.Color> additional = new GenericList<Avalonia.Media.Color>();

        for (int i = 0; i < pixels.Length; i++)
        {
            Avalonia.Media.Color p = pixels.Get(i);

            if (IsInRange(p.R - this._colorRange, p.R + this._colorRange, this._brightestColor.R) &&
                IsInRange(p.G - this._colorRange, p.G + this._colorRange, this._brightestColor.G) &&
                IsInRange(p.B - this._colorRange, p.B + this._colorRange, this._brightestColor.B))
            {
                additional.Add(p);
            }
        }

        double r = 0;
        double g = 0;
        double b = 0;

        for (int i = 0; i < additional.Length; i++)
        {
            Avalonia.Media.Color pixel = additional.Get(i);

            double red = pixel.R;
            double green = pixel.G;
            double blue = pixel.B;

            if (red > Math.Max(green, blue))
            {
                r += red * this._bigShift;
            }
            else
            {
                r += red * this._smallShift;
            }
            
            if (green > Math.Max(red, blue))
            {
                g += green * this._bigShift;
            }
            else
            {
                g += green * this._smallShift;
            }
            
            if (blue > Math.Max(red, green))
            {
                b += blue * this._bigShift;
            }
            else
            {
                b += blue * this._smallShift;
            }
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

                    if (row % this._pixelSteps == 0 && col % this._pixelSteps == 0)
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