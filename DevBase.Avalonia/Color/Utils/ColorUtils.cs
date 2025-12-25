using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Utils;

using Color = global::Avalonia.Media.Color;

/// <summary>
/// Provides utility methods for handling colors.
/// </summary>
public class ColorUtils
{
    /// <summary>
    /// Extracts all pixels from a bitmap as a list of colors.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>A list of colors, excluding fully transparent ones.</returns>
    public static AList<Color> GetPixels(Bitmap bitmap)
    {
        using MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream);
        
        memoryStream.Seek(0, SeekOrigin.Begin);
            
        WriteableBitmap writeableBitmap = WriteableBitmap.Decode(memoryStream);
        using ILockedFramebuffer? lockedBitmap = writeableBitmap.Lock();

        Color[] colors = new Color[writeableBitmap.PixelSize.Height * writeableBitmap.PixelSize.Width];
        
        for (int y = 0; y < writeableBitmap.PixelSize.Height; y++)
        {
            for (int x = 0; x < writeableBitmap.PixelSize.Width; x++)
            {
                Span<byte> pixel = lockedBitmap.GetPixel(x, y);

                if (pixel.Length != 4)
                    continue;
                    
                byte blue = pixel[0];
                byte green = pixel[1];
                byte red = pixel[2];
                byte alpha = pixel[3];

                colors[y + x] = new Color(alpha, red, green, blue);
            }
        }

        return colors.RemoveNullValues().ToAList();
    }
}