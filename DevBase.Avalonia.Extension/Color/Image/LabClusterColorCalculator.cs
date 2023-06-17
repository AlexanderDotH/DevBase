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

public class LabClusterColorCalculator
{
    public double SmallShift { get; set; } = 1.0d;
    
    public double BigShift { get; set; } = 1.0d;
    
    public double Tolerance { get; set; } = 1E-05d;
    
    public int Clusters { get; set; } = 10;

    public int MaxRange { get; set; } = 5;

    public bool UsePredefinedSet { get; set; } = true;

    public bool AllowEdgeCase { get; set; } = false;
    
    public PreProcessingConfiguration PreProcessing { get; set; } =
        new PreProcessingConfiguration()
        {
            BlurSigma = 5f,
            BlurRounds = 10,
            BlurPreProcessing = false
        };
    
    public FilterConfiguration Filter { get; set; } =
        new FilterConfiguration()
        {
            MinChroma = 30d,
            MinBrightness = 10d,
            FilterChroma = true,
            FilterBrightness = true
        };

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


    public AList<LabColor> AdditionalColorDataset { get; set; } = new AList<LabColor>();

    public LabClusterColorCalculator()
    {
        this._converter = new RGBToLabConverter();
    }

    public Color GetColorFromBitmap(IBitmap bitmap)
    {
        (KMeansClusterCollection, IOrderedEnumerable<IGrouping<int, int>>) clusters = ClusterCalculation(bitmap);
        return GetRangeAndCalcAverage(clusters.Item1, clusters.Item2, this.MaxRange);
    }

    public AList<Color> GetColorListFromBitmap(IBitmap bitmap)
    {
        (KMeansClusterCollection, IOrderedEnumerable<IGrouping<int, int>>) clusters = ClusterCalculation(bitmap);
        return GetRange(clusters.Item1, clusters.Item2, this.MaxRange);
    }

    private (KMeansClusterCollection, IOrderedEnumerable<IGrouping<int, int>>) ClusterCalculation(IBitmap bitmap)
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
        
        if (this.Filter.FilterChroma)
            dominantColorSet.AddRange(colors.FilterChroma(this.Filter.MinChroma));
        
        if (this.Filter.FilterBrightness)
            dominantColorSet.AddRange(colors.FilterBrightness(this.Filter.MinBrightness));
        
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