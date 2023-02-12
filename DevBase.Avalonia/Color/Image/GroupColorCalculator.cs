using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Image;

public class GroupColorCalculator
{
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    private int _brightness;
    
    public GroupColorCalculator()
    {
        this._colorRange = 70;
        this._bigShift = 1.3;
        this._smallShift = 1;
        this._pixelSteps = 10;
        this._brightness = 20;
    }

    public GroupColorCalculator(double bigShift, double smallShift) : this()
    {
        this._bigShift = bigShift;
        this._smallShift = smallShift;
    }
    
    public global::Avalonia.Media.Color GetColorFromBitmap(IBitmap bitmap)
    {
        ATupleList<global::Avalonia.Media.Color, AList<global::Avalonia.Media.Color>> colorGroups = GetColorGroups(bitmap);

        if (colorGroups.IsEmpty())
            return new  global::Avalonia.Media.Color();
        
        AList< global::Avalonia.Media.Color> biggestGroup = colorGroups.Get(0).Item2;

        for (int i = 0; i < colorGroups.Length; i++)
        {
            var element = colorGroups.Get(i);

            if (element.Item2.Length > biggestGroup.Length)
            {
                biggestGroup = element.Item2;
            }
        }
        
        double r = 0;
        double g = 0;
        double b = 0;

        for (int i = 0; i < biggestGroup.Length; i++)
        {
            global::Avalonia.Media.Color pixel = biggestGroup.Get(i);

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

        r /= biggestGroup.Length;
        g /= biggestGroup.Length;
        b /= biggestGroup.Length;

        return CorrectColor(r, g, b);
    }

    private global::Avalonia.Media.Color CorrectColor(double r, double g, double b)
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

        return new global::Avalonia.Media.Color(255, rB, gB, bB);
    }

    private bool IsInRange(double min, double max, double current)
    {
        return min < current && max > current;
    }

    private global::Avalonia.Media.Color FindNearestColor(
        ATupleList<global::Avalonia.Media.Color, AList<global::Avalonia.Media.Color>> colorGroups, global::Avalonia.Media.Color color)
    {
        for (int i = 0; i < colorGroups.Length; i++)
        {
            var entry = colorGroups.Get(i);

            global::Avalonia.Media.Color c = entry.Item1;

            if (IsInRange(c.R - this._colorRange, c.R + this._colorRange, color.R) &&
                IsInRange(c.G - this._colorRange, c.G + this._colorRange, color.G) &&
                IsInRange(c.B - this._colorRange, c.B + this._colorRange, color.B))
            {
                return entry.Item1;
            }
        }

        return new global::Avalonia.Media.Color();
    }

    private ATupleList<global::Avalonia.Media.Color, AList<global::Avalonia.Media.Color>> GetColorGroups(IBitmap bitmap)
    {
        ATupleList<global::Avalonia.Media.Color, AList<global::Avalonia.Media.Color>> colorGroups = new ATupleList<global::Avalonia.Media.Color, AList<global::Avalonia.Media.Color>>();
        
        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            
            WriteableBitmap writeableBitmap = WriteableBitmap.Decode(memoryStream);
            using var lockedBitmap = writeableBitmap.Lock();

            for (int y = 0; y < writeableBitmap.PixelSize.Height; y++)
            {
                for (int x = 0; x < writeableBitmap.PixelSize.Width; x++)
                {
                    var pixel = lockedBitmap.GetPixel(x, y);

                    if (pixel.Length != 4)
                        continue;
                    
                    byte red = pixel[0];
                    byte green = pixel[1];
                    byte blue = pixel[2];
                    byte alpha = pixel[3];
                    
                    if (x % this._pixelSteps == 0 && y % this._pixelSteps == 0)
                    {
                        global::Avalonia.Media.Color c = new global::Avalonia.Media.Color(alpha, red, green, blue);
                        
                        if (red < this._brightness || green < this._brightness || blue < this._brightness)
                        {
                            continue;
                        }
                        
                        global::Avalonia.Media.Color col1 = FindNearestColor(colorGroups, c);

                        AList<global::Avalonia.Media.Color> c1 = colorGroups.FindEntrySafe(col1);

                        if (c1 == null)
                        {
                            AList<global::Avalonia.Media.Color> newColorList = new AList<global::Avalonia.Media.Color>();
                            newColorList.Add(c);
                            colorGroups.Add(c, newColorList);
                        }
                        else
                        {
                            c1.Add(c);
                            colorGroups.Add(col1, c1);
                        }
                    }
                }
            }
        }

        return colorGroups;
    }

    public double ColorRange
    {
        get => _colorRange;
        set => _colorRange = value;
    }

    public double BigShift
    {
        get => _bigShift;
        set => _bigShift = value;
    }

    public double SmallShift
    {
        get => _smallShift;
        set => _smallShift = value;
    }

    public int PixelSteps
    {
        get => _pixelSteps;
        set => _pixelSteps = value;
    }

    public int Brightness
    {
        get => _brightness;
        set => _brightness = value;
    }
}