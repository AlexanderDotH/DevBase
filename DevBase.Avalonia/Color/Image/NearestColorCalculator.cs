using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Image;

/// <summary>
/// Calculates the nearest color based on difference logic.
/// </summary>
public class NearestColorCalculator
{
    private global::Avalonia.Media.Color _smallestDiff;
    private  global::Avalonia.Media.Color _brightestColor;

    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NearestColorCalculator"/> class with default settings.
    /// </summary>
    public NearestColorCalculator()
    {
        this._smallestDiff = new  global::Avalonia.Media.Color();

        this._colorRange = 30;
        this._bigShift = 2.6;
        this._smallShift = 2.8;
        this._pixelSteps = 10;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NearestColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public NearestColorCalculator(double bigShift, double smallShift) : this()
    {
        this._bigShift = bigShift;
        this._smallShift = smallShift;
    }
    
    /// <summary>
    /// Calculates the nearest color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated color.</returns>
    public unsafe global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    {
        AList<global::Avalonia.Media.Color> pixels = GetPixels(bitmap);

        double r = 0;
        double g = 0;
        double b = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            global::Avalonia.Media.Color pixel = pixels.Get(i);

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

        r /= pixels.Length;
        g /= pixels.Length;
        b /= pixels.Length;

        r *= 0.5;
        g *= 0.5;
        b *= 0.5;

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

    private int CalculateDiff(int r1, int g1, int b1, int r2, int g2, int b2)
    {
        int diffR = Math.Abs(r1 - r2);
        int diffG = Math.Abs(g1 - g2);
        int diffB = Math.Abs(b1 - b2);
        return (diffR + diffG + diffB);
    }
    
    private AList<global::Avalonia.Media.Color> GetPixels(Bitmap bitmap)
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

                    if (!(x % this._pixelSteps == 0 && y % this._pixelSteps == 0))
                    {
                        continue;
                    }

                    if (red < brightness || green < brightness || blue < brightness)
                    {
                        continue;
                    }
                    
                    if (colors.IsEmpty())
                    {
                        global::Avalonia.Media.Color color = new global::Avalonia.Media.Color(255, red, green, blue);
                        colors.Add(color);
                    }
                    else 
                    {
                        for (int i = 0; i < colors.Length; i++)
                        {
                            global::Avalonia.Media.Color color = colors.Get(i);
                            
                            int diff = CalculateDiff(color.R, color.G, color.B, red, green, blue);

                            int colorSize = (color.R + color.G + color.B);
                            int otherColorSize = (red + green + blue);
                            
                            if (diff > 100 && colorSize > otherColorSize)
                            {
                                colors.SafeRemove(color);
                            }
                            
                            if (diff <= 30)
                            {
                                global::Avalonia.Media.Color currentColor = new global::Avalonia.Media.Color(255, red, green, blue);

                                if (!colors.SafeContains(currentColor))
                                {
                                    colors.Add(currentColor);
                                }
                            }
                        }
                    }

                }
            }
        }

        return colors;
    }

    /// <summary>
    /// Gets or sets the color with the smallest difference found.
    /// </summary>
    public global::Avalonia.Media.Color SmallestDiff
    {
        get => _smallestDiff;
        set => _smallestDiff = value;
    }

    /// <summary>
    /// Gets or sets the range within which colors are considered similar.
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