using Accord.MachineLearning;
using Avalonia.Media.Imaging;
using Colourful;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Color.Utils;
using DevBase.Avalonia.Data;
using DevBase.Avalonia.Extension.Configuration;
using DevBase.Avalonia.Extension.Converter;
using DevBase.Avalonia.Extension.Extension;
using DevBase.Avalonia.Extension.Processing;
using DevBase.Extensions;
using DevBase.Generics;

namespace DevBase.Avalonia.Extension.Color.Image;

using Color = global::Avalonia.Media.Color;

/// <summary>
/// Calculates dominant colors from an image using KMeans clustering on Lab values.
/// This is the preferred calculator for better color accuracy closer to human perception.
/// </summary>
public class LabClusterColorCalculator
{
    /// <summary>
    /// Gets or sets the small shift value for post-processing.
    /// </summary>
    public double SmallShift { get; set; } = 1.0d;
    
    /// <summary>
    /// Gets or sets the big shift value for post-processing.
    /// </summary>
    public double BigShift { get; set; } = 1.0d;
    
    /// <summary>
    /// Gets or sets the tolerance for KMeans clustering.
    /// </summary>
    public double Tolerance { get; set; } = 1E-05d;
    
    /// <summary>
    /// Gets or sets the number of clusters to find.
    /// </summary>
    public int Clusters { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum range of clusters to consider for the result.
    /// </summary>
    public int MaxRange { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether to use a predefined dataset of colors.
    /// </summary>
    public bool UsePredefinedSet { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to return a fallback result if filtering removes all colors.
    /// </summary>
    public bool AllowEdgeCase { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the pre-processing configuration (e.g. blur).
    /// </summary>
    public PreProcessingConfiguration PreProcessing { get; set; } =
        new PreProcessingConfiguration()
        {
            BlurSigma = 5f,
            BlurRounds = 10,
            BlurPreProcessing = false
        };
    
    /// <summary>
    /// Gets or sets the filtering configuration (chroma, brightness).
    /// </summary>
    public FilterConfiguration Filter { get; set; } =
        new FilterConfiguration()
        {
            ChromaConfiguration = new ChromaConfiguration()
            {
                FilterChroma = true,
                MinChroma = 30d,
                MaxChroma = 100d
            },
            BrightnessConfiguration = new BrightnessConfiguration()
            {
                FilterBrightness = true,
                MinBrightness = 10d,
                MaxBrightness = 100d
            }
        };

    /// <summary>
    /// Gets or sets the post-processing configuration (pastel, shifting).
    /// </summary>
    public PostProcessingConfiguration PostProcessing { get; set; } =
        new PostProcessingConfiguration()
        {
            SmallShift = 1.0d,
            BigShift = 1.0d,
            ColorShiftingPostProcessing = false,
            PastelGuidance = 30,
            PastelLightness = 30,
            PastelLightnessSubtractor = 20,
            PastelSaturation = 0.8,
            PastelPostProcessing = true
        };
    
    private RGBToLabConverter _converter;


    /// <summary>
    /// Gets or sets additional Lab colors to include in the clustering dataset.
    /// </summary>
    public AList<LabColor> AdditionalColorDataset { get; set; } = new AList<LabColor>();

    /// <summary>
    /// Initializes a new instance of the <see cref="LabClusterColorCalculator"/> class.
    /// </summary>
    public LabClusterColorCalculator()
    {
        this._converter = new RGBToLabConverter();
    }

    /// <summary>
    /// Calculates the dominant color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public Color GetColorFromBitmap(Bitmap bitmap)
    {
        (KMeansClusterCollection, IOrderedEnumerable<IGrouping<int, int>>) clusters = ClusterCalculation(bitmap);
        return GetRangeAndCalcAverage(clusters.Item1, clusters.Item2, this.MaxRange);
    }

    /// <summary>
    /// Calculates a list of dominant colors from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>A list of calculated colors.</returns>
    public AList<Color> GetColorListFromBitmap(Bitmap bitmap)
    {
        (KMeansClusterCollection, IOrderedEnumerable<IGrouping<int, int>>) clusters = ClusterCalculation(bitmap);
        return GetRange(clusters.Item1, clusters.Item2, this.MaxRange);
    }

    private (KMeansClusterCollection, IOrderedEnumerable<IGrouping<int, int>>) ClusterCalculation(Bitmap bitmap)
    {
        if (this.PreProcessing.BlurPreProcessing)
        {
            ImagePreProcessor preProcessor =
                new ImagePreProcessor(this.PreProcessing.BlurSigma, this.PreProcessing.BlurRounds);
            bitmap = preProcessor.Process(bitmap);
        }
        
        AList<LabColor> pixels = ColorUtils.GetPixels(bitmap).ToRgbColor().ToLabColor(this._converter);

        double[][] colors = pixels.GetAsArray().Select(x => new[] { x.L, x.a, x.b }).ToArray();
        KMeansClusterCollection cluster = InitCluster(pixels);

        AList<int> allowedCluster = new AList<int>(cluster.Decide(colors));

        IOrderedEnumerable<IGrouping<int, int>> mostCommonCluster = allowedCluster.GetAsArray().GroupBy(x => x).OrderByDescending(x => x.Count());
        return (cluster, mostCommonCluster);
    }
    
    private KMeansClusterCollection InitCluster(AList<LabColor> colors)
    {
        AList<LabColor> dominantColorSet = new AList<LabColor>();
        
        if (this.Filter.ChromaConfiguration.FilterChroma)
            dominantColorSet.AddRange(colors.FilterChroma(
                this.Filter.ChromaConfiguration.MinChroma, 
                this.Filter.ChromaConfiguration.MaxChroma));
        
        if (this.Filter.BrightnessConfiguration.FilterBrightness)
            dominantColorSet.AddRange(colors.FilterBrightness(
                this.Filter.BrightnessConfiguration.MinBrightness, 
                this.Filter.BrightnessConfiguration.MaxBrightness));
        
        if (this.UsePredefinedSet)
            dominantColorSet.AddRange(ClusterData.RGB_DATA.ToAList().ToRgbColor().ToLabColor(this._converter));
        
        dominantColorSet.AddRange(AdditionalColorDataset);

        // Edge-Case when everything got filtered out, ist just always want a result
        if (this.AllowEdgeCase)
            if (dominantColorSet.Length == 0)
                dominantColorSet.AddRange(colors.GetAsArray().RemoveNullValues());
        
        double[][] initialCentroids = dominantColorSet.GetAsArray().Select(x => new[] { x.L, x.a, x.b }).ToArray();

        KMeans means = new KMeans(k: this.Clusters)
        {
            Tolerance = this.Tolerance
        };
        
        KMeansClusterCollection collection = means.Learn(initialCentroids);
        return collection;
    }

    private Color GetRangeAndCalcAverage(KMeansClusterCollection cluster, IOrderedEnumerable<IGrouping<int, int>> clusters, int max)
    {
        return GetRange(cluster, clusters, max).Average();
    }
    
    private AList<Color> GetRange(KMeansClusterCollection cluster, IOrderedEnumerable<IGrouping<int, int>> clusters, int max)
    {
        List<IGrouping<int, int>> list = new List<IGrouping<int, int>>();

        var clusterList = clusters.ToList();
        list.AddRange(clusterList.GetRange(0, Math.Min(clusterList.Count, max)));

        return PostProcess(cluster, list);
    }

    private AList<Color> PostProcess(KMeansClusterCollection cluster, IList<IGrouping<int, int>> elements)
    {
        AList<Color> colors = new AList<Color>();

        for (var i = 0; i < elements.Count; i++)
        {
            LabColor color = ClusterToLab(cluster, elements[i].Key);

            if (this.PostProcessing.PastelPostProcessing)
            {
                if (color.L < this.PostProcessing.PastelGuidance)
                {
                    color = color.ToPastel(this.PostProcessing.PastelLightness, 
                        this.PostProcessing.PastelSaturation);
                }
                else
                {
                    color = color.ToPastel(
                        Math.Abs(this.PostProcessing.PastelLightness - 
                                 this.PostProcessing.PastelLightnessSubtractor), 
                        this.PostProcessing.PastelSaturation);
                }
            }

            if (this.PostProcessing.ColorShiftingPostProcessing)
            {
                color = color
                    .ToRgbColor(this._converter)
                    .DeNormalize()
                    .Shift(this.SmallShift, this.BigShift)
                    .Normalize()
                    .ToRgbColor().ToLabColor(this._converter);
            }

            colors.Add(color.ToRgbColor(this._converter).DeNormalize());
        }

        return colors;
    }

    private LabColor ClusterToLab(KMeansClusterCollection cluster, int clusterID)
    {
        double[] dominantColor = cluster.Centroids[clusterID];
        return new LabColor(dominantColor[0], dominantColor[1], dominantColor[2]);
    }
}