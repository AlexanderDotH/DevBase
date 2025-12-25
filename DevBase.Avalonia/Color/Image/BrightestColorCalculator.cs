using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Image;

/// <summary>
/// Calculates the brightest color from a bitmap.
/// </summary>
public class BrightestColorCalculator
{
    private global::Avalonia.Media.Color _brightestColor;
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BrightestColorCalculator"/> class with default settings.
    /// </summary>
    public BrightestColorCalculator()
    {
        this._brightestColor = new global::Avalonia.Media.Color();

        this._colorRange = 50;
        this._bigShift = 1.5;
        this._smallShift = 0.5;
        this._pixelSteps = 10;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrightestColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public BrightestColorCalculator(double bigShift, double smallShift) : this()
    {
        this._bigShift = bigShift;
        this._smallShift = smallShift;
    }
    
    /// <summary>
    /// Calculates the brightest color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated brightest color.</returns>
    public unsafe global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    {
        AList<global::Avalonia.Media.Color> pixels = GetPixels(bitmap);
        AList<global::Avalonia.Media.Color> additional = new AList<global::Avalonia.Media.Color>();

        for (int i = 0; i < pixels.Length; i++)
        {
            global::Avalonia.Media.Color p = pixels.Get(i);

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
            global::Avalonia.Media.Color pixel = additional.Get(i);

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
    
    private unsafe AList<global::Avalonia.Media.Color> GetPixels(Bitmap bitmap)
    {
        AList<global::Avalonia.Media.Color> colors = new AList<global::Avalonia.Media.Color>();
        
        double brightness = 0;
       
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

                    double b = (red / 255.0) * 0.3 + (green / 255.0) * 0.59 + (blue / 255.0) * 0.11;

                    if (b > brightness)
                    {
                        brightness = b;
                        this._brightestColor = new  global::Avalonia.Media.Color(alpha, red, green, blue);
                    }

                    if (x % this._pixelSteps == 0 && y % this._pixelSteps == 0)
                    {
                        colors.Add(new global::Avalonia.Media.Color(alpha, red, green, blue));
                    }
                }
            }
        }

        return colors;
    }

    /// <summary>
    /// Gets or sets the range within which colors are considered similar to the brightest color.
    /// </summary>
    public double ColorRange
    {
        get => _colorRange;
        set => _colorRange = value;
    }

    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift
    {
        get => _bigShift;
        set => _bigShift = value;
    }

    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift
    {
        get => _smallShift;
        set => _smallShift = value;
    }

    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps
    {
        get => _pixelSteps;
        set => _pixelSteps = value;
    }
}