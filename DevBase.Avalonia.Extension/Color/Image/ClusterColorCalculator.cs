using Accord.MachineLearning;
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Color.Utils;
using DevBase.Avalonia.Data;
using DevBase.Generics;

namespace DevBase.Avalonia.Extension.Color.Image;

using Color = global::Avalonia.Media.Color;

/// <summary>
/// Calculates dominant colors from an image using KMeans clustering on RGB values.
/// </summary>
[Obsolete("Use LabClusterColorCalculator instead")]
public class ClusterColorCalculator
{
    /// <summary>
    /// Gets or sets the minimum saturation threshold for filtering colors.
    /// </summary>
    public double MinSaturation { get; set; } = 50d;
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold for filtering colors.
    /// </summary>
    public double MinBrightness { get; set; } = 70d;
    
    /// <summary>
    /// Gets or sets the small shift value.
    /// </summary>
    public double SmallShift { get; set; } = 1.0d;
    
    /// <summary>
    /// Gets or sets the big shift value.
    /// </summary>
    public double BigShift { get; set; } = 1.0d;
    
    /// <summary>
    /// Gets or sets the tolerance for KMeans clustering.
    /// </summary>
    public double Tolerance { get; set; } = 0.5d;
    
    /// <summary>
    /// Gets or sets the number of clusters to find.
    /// </summary>
    public int Clusters { get; set; } = 20;
    
    /// <summary>
    /// Gets or sets the maximum range of clusters to consider for the result.
    /// </summary>
    public int MaxRange { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether to use a predefined dataset.
    /// </summary>
    public bool PredefinedDataset { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to filter by saturation.
    /// </summary>
    public bool FilterSaturation { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to filter by brightness.
    /// </summary>
    public bool FilterBrightness { get; set; } = true;

    /// <summary>
    /// Gets or sets additional colors to include in the clustering dataset.
    /// </summary>
    public AList<Color> AdditionalColorDataset { get; set; } = new AList<Color>();

    /// <summary>
    /// Calculates the dominant color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public Color GetColorFromBitmap(Bitmap bitmap)
    {
        AList<Color> pixels = ColorUtils.GetPixels(bitmap);

        double[][] colors = pixels.GetAsArray().Select(x => new double[] { x.R, x.G, x.B }).ToArray();
        
        KMeansClusterCollection cluster = InitCluster(pixels);

        AList<int> allowedCluster = new AList<int>(cluster.Decide(colors));

        IOrderedEnumerable<IGrouping<int, int>> mostCommonCluster = allowedCluster.GetAsArray().GroupBy(x => x).OrderByDescending(x => x.Count());

        return GetRangeAndCalcAverage(cluster, mostCommonCluster, this.MaxRange);
    }

    private KMeansClusterCollection InitCluster(AList<Color> colors)
    {
        AList<Color> dominantColorSet = new AList<Color>();
        
        if (this.PredefinedDataset)
            dominantColorSet.AddRange(ClusterData.RGB_DATA);
        
        if (this.FilterSaturation)
            dominantColorSet.AddRange(colors.FilterSaturation(MinSaturation));
        
        if (this.FilterBrightness)
            dominantColorSet.AddRange(colors.FilterBrightness(MinBrightness));
        
        dominantColorSet.AddRange(AdditionalColorDataset);

        double[][] initialCentroids = dominantColorSet.GetAsArray().Select(x => new double[] { x.R, x.G, x.B }).ToArray();

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