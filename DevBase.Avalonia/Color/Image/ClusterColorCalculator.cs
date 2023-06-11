using Accord.MachineLearning;
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Image;

using Color = global::Avalonia.Media.Color;

public class ClusterColorCalculator
{
    public global::Avalonia.Media.Color GetColorFromBitmap(IBitmap bitmap)
    {
        AList<global::Avalonia.Media.Color> pixels = GetPixels(bitmap);
    
        double[][] colors = pixels.GetAsArray().Select(x => new double[] { x.R, x.G, x.B }).ToArray();

        KMeans kmeans = new KMeans(k: 1);
        var clusters = kmeans.Learn(colors);

        int[] predicted = clusters.Decide(colors);

        int mostCommonCluster = predicted.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;

        double[] dominantColor = clusters.Centroids[mostCommonCluster];

        byte r = Convert.ToByte(dominantColor[0]);
        byte g = Convert.ToByte(dominantColor[1]);
        byte b = Convert.ToByte(dominantColor[2]);

        return new global::Avalonia.Media.Color(255, r, g, b).Shift(1.0, 1.2);
    }

    private AList<Color> GetPixels(IBitmap bitmap)
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

        return colors;
    }
}