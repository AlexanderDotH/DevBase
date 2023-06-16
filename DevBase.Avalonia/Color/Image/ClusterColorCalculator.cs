using Accord.MachineLearning;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Colourful;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Color.Utils;
using DevBase.Avalonia.Data;
using DevBase.Generics;

namespace DevBase.Avalonia.Color.Image;

using Color = global::Avalonia.Media.Color;

public class ClusterColorCalculator
{
    public double MinChroma { get; set; } = 50d;
    public double MinBrightness { get; set; } = 70d;
    public double SmallShift { get; set; } = 1.0d;
    public double BigShift { get; set; } = 1.0d;
    public double Tolerance { get; set; } = 0.5d;
    public int Clusters { get; set; } = 20;
    public int MaxRange { get; set; } = 5;

    public bool PredefinedDataset { get; set; } = true;
    public bool FilterSaturation { get; set; } = true;
    public bool FilterBrightness { get; set; } = true;

    public AList<LabColor> AdditionalColorDataset { get; set; } = new AList<LabColor>();

    public Color GetColorFromBitmap(IBitmap bitmap)
    {
        AList<LabColor> pixels = ColorUtils.GetPixels(bitmap);

        double[][] colors = pixels.GetAsArray().Select(x => new double[] { x.L, x.a, x.b }).ToArray();
        
        KMeansClusterCollection cluster = InitCluster(pixels);

        AList<int> allowedCluster = new AList<int>(cluster.Decide(colors));

        IOrderedEnumerable<IGrouping<int, int>> mostCommonCluster = allowedCluster.GetAsArray().GroupBy(x => x).OrderByDescending(x => x.Count());

        return GetRangeAndCalcAverage(cluster, mostCommonCluster, this.MaxRange);
    }

    private KMeansClusterCollection InitCluster(AList<LabColor> colors)
    {
        AList<LabColor> dominantColorSet = new AList<LabColor>();
        
        /*
        if (this.PredefinedDataset)
            dominantColorSet.AddRange(ClusterData.DATA);
            */
        
        if (this.FilterSaturation)
            dominantColorSet.AddRange(colors.FilterChroma(MinChroma));
        
        if (this.FilterBrightness)
            dominantColorSet.AddRange(colors.FilterBrightness(MinBrightness));
        
        dominantColorSet.AddRange(AdditionalColorDataset);

        double[][] initialCentroids = dominantColorSet.GetAsArray().Select(x => new double[] { x.L, x.a, x.b }).ToArray();

        KMeans means = new KMeans(k: this.Clusters)
        {
            Tolerance = this.Tolerance
        };
        
        KMeansClusterCollection collection = means.Learn(initialCentroids);
        return collection;
    }

    private Color GetRangeAndCalcAverage(KMeansClusterCollection cluster, IOrderedEnumerable<IGrouping<int, int>> clusters, int max)
    {
        AList<Color> list = new AList<Color>();

        var values = clusters.ToList().GetRange(0, max);
        
        for (var i = 0; i < values.Count; i++)
            list.AddRange(ClusterToColor(cluster, values[i].Key).Shift(this.SmallShift, this.BigShift));

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
}