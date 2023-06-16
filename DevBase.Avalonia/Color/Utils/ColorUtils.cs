using Avalonia.Media.Imaging;
using Colourful;
using DevBase.Avalonia.Color.Converter;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Utils;

using Color = global::Avalonia.Media.Color;

public class ColorUtils
{
    public static AList<LabColor> GetPixels(IBitmap bitmap)
    {
        AList<Color> colors = new AList<Color>();
        
        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            
            WriteableBitmap writeableBitmap = WriteableBitmap.Decode(memoryStream);
            using var lockedBitmap = writeableBitmap.Lock();

            for (int y = 0; y < writeableBitmap.PixelSize.Height; y++)
            {
                Color[] c = new Color[writeableBitmap.PixelSize.Width];
                
                for (int x = 0; x < writeableBitmap.PixelSize.Width; x++)
                {
                    Span<byte> pixel = lockedBitmap.GetPixel(x, y);

                    if (pixel.Length != 4)
                        continue;
                    
                    byte red = pixel[0];
                    byte green = pixel[1];
                    byte blue = pixel[2];
                    byte alpha = pixel[3];

                    c[x] = new Color(alpha, red, green, blue);
                }
                
                colors.AddRange(c);
            }
        }

        RGBToLabConverter converter = new RGBToLabConverter();
        return colors.ToRgbColor().ToLabColor(converter);
    }
}