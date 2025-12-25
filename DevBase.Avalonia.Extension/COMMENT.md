# DevBase.Avalonia.Extension Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Avalonia.Extension project.

## Table of Contents

- [Color](#color)
  - [Image](#image)
    - [ClusterColorCalculator](#clustercolorcalculator)
    - [LabClusterColorCalculator](#labclustercolorcalculator)
- [Configuration](#configuration)
  - [BrightnessConfiguration](#brightnessconfiguration)
  - [ChromaConfiguration](#chromaconfiguration)
  - [FilterConfiguration](#filterconfiguration)
  - [PostProcessingConfiguration](#postprocessingconfiguration)
  - [PreProcessingConfiguration](#preprocessingconfiguration)
- [Converter](#converter)
  - [RGBToLabConverter](#rgbtolabconverter)
- [Extension](#extension)
  - [BitmapExtension](#bitmapextension)
  - [ColorNormalizerExtension](#colornormalizerextension)
  - [LabColorExtension](#labcolorextension)
- [Processing](#processing)
  - [ImagePreProcessor](#imagepreprocessor)

## Color

### Image

#### ClusterColorCalculator

```csharp
/// <summary>
/// Calculates dominant colors from an image using KMeans clustering on RGB values.
/// </summary>
[Obsolete("Use LabClusterColorCalculator instead")]
public class ClusterColorCalculator
{
    /// <summary>
    /// Gets or sets the minimum saturation threshold for filtering colors.
    /// </summary>
    public double MinSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold for filtering colors.
    /// </summary>
    public double MinBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the small shift value.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the tolerance for KMeans clustering.
    /// </summary>
    public double Tolerance { get; set; }
    
    /// <summary>
    /// Gets or sets the number of clusters to find.
    /// </summary>
    public int Clusters { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum range of clusters to consider for the result.
    /// </summary>
    public int MaxRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use a predefined dataset.
    /// </summary>
    public bool PredefinedDataset { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to filter by saturation.
    /// </summary>
    public bool FilterSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to filter by brightness.
    /// </summary>
    public bool FilterBrightness { get; set; }

    /// <summary>
    /// Gets or sets additional colors to include in the clustering dataset.
    /// </summary>
    public AList<Color> AdditionalColorDataset { get; set; }

    /// <summary>
    /// Calculates the dominant color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public Color GetColorFromBitmap(Bitmap bitmap)
}
```

#### LabClusterColorCalculator

```csharp
/// <summary>
/// Calculates dominant colors from an image using KMeans clustering on Lab values.
/// This is the preferred calculator for better color accuracy closer to human perception.
/// </summary>
public class LabClusterColorCalculator
{
    /// <summary>
    /// Gets or sets the small shift value for post-processing.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value for post-processing.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the tolerance for KMeans clustering.
    /// </summary>
    public double Tolerance { get; set; }
    
    /// <summary>
    /// Gets or sets the number of clusters to find.
    /// </summary>
    public int Clusters { get; set; }

    /// <summary>
    /// Gets or sets the maximum range of clusters to consider for the result.
    /// </summary>
    public int MaxRange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use a predefined dataset of colors.
    /// </summary>
    public bool UsePredefinedSet { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to return a fallback result if filtering removes all colors.
    /// </summary>
    public bool AllowEdgeCase { get; set; }
    
    /// <summary>
    /// Gets or sets the pre-processing configuration (e.g. blur).
    /// </summary>
    public PreProcessingConfiguration PreProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets the filtering configuration (chroma, brightness).
    /// </summary>
    public FilterConfiguration Filter { get; set; }

    /// <summary>
    /// Gets or sets the post-processing configuration (pastel, shifting).
    /// </summary>
    public PostProcessingConfiguration PostProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets additional Lab colors to include in the clustering dataset.
    /// </summary>
    public AList<LabColor> AdditionalColorDataset { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabClusterColorCalculator"/> class.
    /// </summary>
    public LabClusterColorCalculator()
    
    /// <summary>
    /// Calculates the dominant color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Calculates a list of dominant colors from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>A list of calculated colors.</returns>
    public AList<Color> GetColorListFromBitmap(Bitmap bitmap)
}
```

## Configuration

### BrightnessConfiguration

```csharp
/// <summary>
/// Configuration for brightness filtering.
/// </summary>
public class BrightnessConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether brightness filtering is enabled.
    /// </summary>
    public bool FilterBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold (0-100).
    /// </summary>
    public double MinBrightness { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum brightness threshold (0-100).
    /// </summary>
    public double MaxBrightness { get; set; }
}
```

### ChromaConfiguration

```csharp
/// <summary>
/// Configuration for chroma (color intensity) filtering.
/// </summary>
public class ChromaConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether chroma filtering is enabled.
    /// </summary>
    public bool FilterChroma { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum chroma threshold.
    /// </summary>
    public double MinChroma { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum chroma threshold.
    /// </summary>
    public double MaxChroma { get; set; }
}
```

### FilterConfiguration

```csharp
/// <summary>
/// Configuration for color filtering settings.
/// </summary>
public class FilterConfiguration
{
    /// <summary>
    /// Gets or sets the chroma configuration.
    /// </summary>
    public ChromaConfiguration ChromaConfiguration { get; set; }
    
    /// <summary>
    /// Gets or sets the brightness configuration.
    /// </summary>
    public BrightnessConfiguration BrightnessConfiguration { get; set; }
}
```

### PostProcessingConfiguration

```csharp
/// <summary>
/// Configuration for post-processing of calculated colors.
/// </summary>
public class PostProcessingConfiguration
{
    /// <summary>
    /// Gets or sets the small shift value for color shifting.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the big shift value for color shifting.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether color shifting post-processing is enabled.
    /// </summary>
    public bool ColorShiftingPostProcessing { get; set; }
    
    /// <summary>
    /// Gets or sets the target lightness for pastel processing.
    /// </summary>
    public double PastelLightness { get; set; }
    
    /// <summary>
    /// Gets or sets the lightness subtractor value for pastel processing when lightness is above guidance.
    /// </summary>
    public double PastelLightnessSubtractor { get; set; }
    
    /// <summary>
    /// Gets or sets the saturation multiplier for pastel processing.
    /// </summary>
    public double PastelSaturation { get; set; }
    
    /// <summary>
    /// Gets or sets the lightness threshold to decide how to adjust pastel lightness.
    /// </summary>
    public double PastelGuidance { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether pastel post-processing is enabled.
    /// </summary>
    public bool PastelPostProcessing { get; set; }
}
```

### PreProcessingConfiguration

```csharp
/// <summary>
/// Configuration for image pre-processing.
/// </summary>
public class PreProcessingConfiguration
{
    /// <summary>
    /// Gets or sets the sigma value for blur.
    /// </summary>
    public float BlurSigma { get; set; }
    
    /// <summary>
    /// Gets or sets the number of blur rounds.
    /// </summary>
    public int BlurRounds { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether blur pre-processing is enabled.
    /// </summary>
    public bool BlurPreProcessing { get; set; }
}
```

## Converter

### RGBToLabConverter

```csharp
/// <summary>
/// Converter for transforming between RGB and LAB color spaces.
/// </summary>
public class RGBToLabConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RGBToLabConverter"/> class.
    /// Configures converters using sRGB working space and D65 illuminant.
    /// </summary>
    public RGBToLabConverter()
    
    /// <summary>
    /// Converts an RGB color to Lab color.
    /// </summary>
    /// <param name="color">The RGB color.</param>
    /// <returns>The Lab color.</returns>
    public LabColor ToLabColor(RGBColor color)
    
    /// <summary>
    /// Converts a Lab color to RGB color.
    /// </summary>
    /// <param name="color">The Lab color.</param>
    /// <returns>The RGB color.</returns>
    public RGBColor ToRgbColor(LabColor color)
}
```

## Extension

### BitmapExtension

```csharp
/// <summary>
/// Provides extension methods for converting between different Bitmap types.
/// </summary>
public static class BitmapExtension
{
    /// <summary>
    /// Converts an Avalonia Bitmap to a System.Drawing.Bitmap.
    /// </summary>
    /// <param name="bitmap">The Avalonia bitmap.</param>
    /// <returns>The System.Drawing.Bitmap.</returns>
    public static Bitmap ToBitmap(this global::Avalonia.Media.Imaging.Bitmap bitmap)
    
    /// <summary>
    /// Converts a System.Drawing.Bitmap to an Avalonia Bitmap.
    /// </summary>
    /// <param name="bitmap">The System.Drawing.Bitmap.</param>
    /// <returns>The Avalonia Bitmap.</returns>
    public static global::Avalonia.Media.Imaging.Bitmap ToBitmap(this Bitmap bitmap)
    
    /// <summary>
    /// Converts a SixLabors ImageSharp Image to an Avalonia Bitmap.
    /// </summary>
    /// <param name="image">The ImageSharp Image.</param>
    /// <returns>The Avalonia Bitmap.</returns>
    public static global::Avalonia.Media.Imaging.Bitmap ToBitmap(this SixLabors.ImageSharp.Image image)
    
    /// <summary>
    /// Converts an Avalonia Bitmap to a SixLabors ImageSharp Image.
    /// </summary>
    /// <param name="bitmap">The Avalonia Bitmap.</param>
    /// <returns>The ImageSharp Image.</returns>
    public static SixLabors.ImageSharp.Image ToImage(this global::Avalonia.Media.Imaging.Bitmap bitmap)
}
```

### ColorNormalizerExtension

```csharp
/// <summary>
/// Provides extension methods for color normalization.
/// </summary>
public static class ColorNormalizerExtension
{
    /// <summary>
    /// Denormalizes an RGBColor (0-1 range) to an Avalonia Color (0-255 range).
    /// </summary>
    /// <param name="normalized">The normalized RGBColor.</param>
    /// <returns>The denormalized Avalonia Color.</returns>
    public static global::Avalonia.Media.Color DeNormalize(this RGBColor normalized)
}
```

### LabColorExtension

```csharp
/// <summary>
/// Provides extension methods for LabColor operations.
/// </summary>
public static class LabColorExtension
{
    /// <summary>
    /// Filters a list of LabColors based on lightness (L) values.
    /// </summary>
    /// <param name="colors">The list of LabColors.</param>
    /// <param name="min">Minimum lightness.</param>
    /// <param name="max">Maximum lightness.</param>
    /// <returns>A filtered list of LabColors.</returns>
    public static AList<LabColor> FilterBrightness(this AList<LabColor> colors, double min, double max)
    
    /// <summary>
    /// Calculates the chroma of a LabColor.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <returns>The chroma value.</returns>
    public static double Chroma(this LabColor color)
    
    /// <summary>
    /// Calculates the chroma percentage relative to a max chroma of 128.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <returns>The chroma percentage.</returns>
    public static double ChromaPercentage(this LabColor color)
    
    /// <summary>
    /// Filters a list of LabColors based on chroma percentage.
    /// </summary>
    /// <param name="colors">The list of LabColors.</param>
    /// <param name="min">Minimum chroma percentage.</param>
    /// <param name="max">Maximum chroma percentage.</param>
    /// <returns>A filtered list of LabColors.</returns>
    public static AList<LabColor> FilterChroma(this AList<LabColor> colors, double min, double max)
    
    /// <summary>
    /// Converts a normalized double array to an RGBColor.
    /// </summary>
    /// <param name="normalized">Normalized array [A, R, G, B] or similar.</param>
    /// <returns>The RGBColor.</returns>
    public static RGBColor ToRgbColor(this double[] normalized)
    
    /// <summary>
    /// Converts an RGBColor to LabColor using the provided converter.
    /// </summary>
    /// <param name="color">The RGBColor.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The LabColor.</returns>
    public static LabColor ToLabColor(this RGBColor color, RGBToLabConverter converter)
    
    /// <summary>
    /// Converts a LabColor to RGBColor using the provided converter.
    /// </summary>
    /// <param name="color">The LabColor.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>The RGBColor.</returns>
    public static RGBColor ToRgbColor(this LabColor color, RGBToLabConverter converter)
    
    /// <summary>
    /// Adjusts a LabColor to be more pastel-like by modifying lightness and saturation.
    /// </summary>
    /// <param name="color">The original LabColor.</param>
    /// <param name="lightness">The lightness to add.</param>
    /// <param name="saturation">The saturation multiplier.</param>
    /// <returns>The pastel LabColor.</returns>
    public static LabColor ToPastel(this LabColor color, double lightness = 20.0d, double saturation = 0.5d)
    
    /// <summary>
    /// Converts a list of Avalonia Colors to RGBColors.
    /// </summary>
    /// <param name="color">The list of Avalonia Colors.</param>
    /// <returns>A list of RGBColors.</returns>
    public static AList<RGBColor> ToRgbColor(this AList<global::Avalonia.Media.Color> color)
    
    /// <summary>
    /// Converts a list of RGBColors to LabColors using the provided converter.
    /// </summary>
    /// <param name="colors">The list of RGBColors.</param>
    /// <param name="converter">The converter instance.</param>
    /// <returns>A list of LabColors.</returns>
    public static AList<LabColor> ToLabColor(this AList<RGBColor> colors, RGBToLabConverter converter)
    
    /// <summary>
    /// Removes default LabColor (0,0,0) values from an array.
    /// </summary>
    /// <param name="colors">The source array.</param>
    /// <returns>An array with default values removed.</returns>
    public static LabColor[] RemoveNullValues(this LabColor[] colors)
}
```

## Processing

### ImagePreProcessor

```csharp
/// <summary>
/// Provides image pre-processing functionality, such as blurring.
/// </summary>
public class ImagePreProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImagePreProcessor"/> class.
    /// </summary>
    /// <param name="sigma">The Gaussian blur sigma value.</param>
    /// <param name="rounds">The number of blur iterations.</param>
    public ImagePreProcessor(float sigma, int rounds = 10)
    
    /// <summary>
    /// Processes an Avalonia Bitmap by applying Gaussian blur.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The processed bitmap.</returns>
    public global::Avalonia.Media.Imaging.Bitmap Process(global::Avalonia.Media.Imaging.Bitmap bitmap)
}
```
