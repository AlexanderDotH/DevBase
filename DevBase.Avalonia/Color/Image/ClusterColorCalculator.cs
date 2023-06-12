using Accord.MachineLearning;
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Color.Utils;
using DevBase.Avalonia.Data;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Image;

using Color = global::Avalonia.Media.Color;

public class ClusterColorCalculator
{

    public Color GetColorFromBitmap(IBitmap bitmap)
    {
        AList<Color> pixels = ColorUtils.GetPixels(bitmap);

        double[][] colors = pixels.GetAsArray().Select(x => new double[] { x.R, x.G, x.B }).ToArray();
        
        KMeansClusterCollection cluster = InitCluster(pixels);

        AList<int> allowedCluster = new AList<int>(cluster.Decide(colors));

        var mostCommonCluster = allowedCluster.GetAsArray().GroupBy(x => x).OrderByDescending(x => x.Count());

        AList<Color> list = new AList<Color>();

        var most = mostCommonCluster.ToList().GetRange(0, 10);
        
        for (var i = 0; i < most.Count; i++)
        {
            list.AddRange(ClusterToColor(cluster, most[i].Key).Shift(1, 1));
        }

        return list.Average();
    }

    private Color ClusterToColor(KMeansClusterCollection cluster, int clusterID)
    {
        double[] dominantColor = cluster.Centroids[clusterID];

        byte r = Convert.ToByte(dominantColor[0]);
        byte g = Convert.ToByte(dominantColor[1]);
        byte b = Convert.ToByte(dominantColor[2]);

        return new Color(255, r, g, b);
    }
    
    private KMeansClusterCollection InitCluster(AList<Color> colors)
    {
        AList<Color> dominantColorSet = new AList<Color>();
        
        dominantColorSet.AddRange(ClusterData.DATA);
        dominantColorSet.AddRange(colors.FilterSaturation(80d));
        dominantColorSet.AddRange(colors.FilterBrightness(90d));

        double[][] initialCentroids = dominantColorSet.GetAsArray().Select(x => new double[] { x.R, x.G, x.B }).ToArray();

        KMeans means = new KMeans(k: 20)
        {
            Tolerance = 0.5
        };
        
        KMeansClusterCollection collection = means.Learn(initialCentroids);
        return collection;
    }
    
}