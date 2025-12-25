# DevBase.Avalonia Project Documentation

This document contains all class, method, and field signatures with their corresponding comments for the DevBase.Avalonia project.

## Table of Contents

- [Color](#color)
  - [Extensions](#extensions)
    - [ColorExtension](#colorextension)
    - [ColorNormalizerExtension](#colornormalizerextension)
    - [LockedFramebufferExtensions](#lockedframebufferextensions)
  - [Image](#image)
    - [BrightestColorCalculator](#brightestcolorcalculator)
    - [GroupColorCalculator](#groupcolorcalculator)
    - [NearestColorCalculator](#nearestcolorcalculator)
  - [Utils](#utils)
    - [ColorUtils](#colorutils)
- [Data](#data)
  - [ClusterData](#clusterdata)

## Color

### Extensions

#### ColorExtension

```csharp
/// <summary>
/// Provides extension methods for <see cref="global::Avalonia.Media.Color"/>.
/// </summary>
public static class ColorExtension
{
    /// <summary>
    /// Shifts the RGB components of the color based on their relative intensity.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    /// <param name="bigShift">The multiplier for the dominant color component.</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/> with shifted values.</returns>
    public static global::Avalonia.Media.Color Shift(
        this global::Avalonia.Media.Color color, 
        double smallShift,
        double bigShift)
    
    /// <summary>
    /// Adjusts the brightness of the color by a percentage.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <param name="percentage">The percentage to adjust brightness (e.g., 50 for 50%).</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/> with adjusted brightness.</returns>
    public static global::Avalonia.Media.Color AdjustBrightness(
        this global::Avalonia.Media.Color color,
        double percentage)
    
    /// <summary>
    /// Calculates the saturation of the color (0.0 to 1.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The saturation value.</returns>
    public static double Saturation(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the saturation percentage of the color (0.0 to 100.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The saturation percentage.</returns>
    public static double SaturationPercentage(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the brightness of the color using weighted RGB values.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The brightness value.</returns>
    public static double Brightness(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the brightness percentage of the color (0.0 to 100.0).
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>The brightness percentage.</returns>
    public static double BrightnessPercentage(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the similarity between two colors as a percentage.
    /// </summary>
    /// <param name="color">The first color.</param>
    /// <param name="otherColor">The second color.</param>
    /// <returns>The similarity percentage (0.0 to 100.0).</returns>
    public static double Similarity(this global::Avalonia.Media.Color color, global::Avalonia.Media.Color otherColor)
    
    /// <summary>
    /// Corrects the color component values to ensure they are within the valid range (0-255).
    /// </summary>
    /// <param name="color">The color to correct.</param>
    /// <returns>A corrected <see cref="global::Avalonia.Media.Color"/>.</returns>
    public static global::Avalonia.Media.Color Correct(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Calculates the average color from a list of colors.
    /// </summary>
    /// <param name="colors">The list of colors.</param>
    /// <returns>The average color.</returns>
    public static global::Avalonia.Media.Color Average(this AList<global::Avalonia.Media.Color> colors)
    
    /// <summary>
    /// Filters a list of colors, returning only those with saturation greater than the specified value.
    /// </summary>
    /// <param name="colors">The source list of colors.</param>
    /// <param name="value">The minimum saturation percentage threshold.</param>
    /// <returns>A filtered list of colors.</returns>
    public static AList<global::Avalonia.Media.Color> FilterSaturation(this AList<global::Avalonia.Media.Color> colors, double value)
    
    /// <summary>
    /// Filters a list of colors, returning only those with brightness greater than the specified percentage.
    /// </summary>
    /// <param name="colors">The source list of colors.</param>
    /// <param name="percentage">The minimum brightness percentage threshold.</param>
    /// <returns>A filtered list of colors.</returns>
    public static AList<global::Avalonia.Media.Color> FilterBrightness(this AList<global::Avalonia.Media.Color> colors, double percentage)
    
    /// <summary>
    /// Removes transparent colors (alpha=0, rgb=0) from the array.
    /// </summary>
    /// <param name="colors">The source array of colors.</param>
    /// <returns>A new array with null/empty values removed.</returns>
    public static global::Avalonia.Media.Color[] RemoveNullValues(this global::Avalonia.Media.Color[] colors)
}
```

#### ColorNormalizerExtension

```csharp
/// <summary>
/// Provides extension methods for normalizing color values.
/// </summary>
public static class ColorNormalizerExtension
{
    /// <summary>
    /// Normalizes the color components to a range of 0.0 to 1.0.
    /// </summary>
    /// <param name="color">The source color.</param>
    /// <returns>An array containing normalized [A, R, G, B] values.</returns>
    public static double[] Normalize(this global::Avalonia.Media.Color color)
    
    /// <summary>
    /// Denormalizes an array of [A, R, G, B] (or [R, G, B]) values back to a Color.
    /// </summary>
    /// <param name="normalized">The normalized color array (values 0.0 to 1.0).</param>
    /// <returns>A new <see cref="global::Avalonia.Media.Color"/>.</returns>
    public static global::Avalonia.Media.Color DeNormalize(this double[] normalized)
}
```

#### LockedFramebufferExtensions

```csharp
/// <summary>
/// Provides extension methods for accessing pixel data from a <see cref="ILockedFramebuffer"/>.
/// </summary>
public static class LockedFramebufferExtensions
{
    /// <summary>
    /// Gets the pixel data at the specified coordinates as a span of bytes.
    /// </summary>
    /// <param name="framebuffer">The locked framebuffer.</param>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>A span of bytes representing the pixel.</returns>
    public static Span<byte> GetPixel(this ILockedFramebuffer framebuffer, int x, int y)
}
```

### Image

#### BrightestColorCalculator

```csharp
/// <summary>
/// Calculates the brightest color from a bitmap.
/// </summary>
public class BrightestColorCalculator
{
    private global::Avalonia.Media.Color _brightestColor;
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BrightestColorCalculator"/> class with default settings.
    /// </summary>
    public BrightestColorCalculator()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BrightestColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public BrightestColorCalculator(double bigShift, double smallShift)
    
    /// <summary>
    /// Calculates the brightest color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated brightest color.</returns>
    public unsafe global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Gets or sets the range within which colors are considered similar to the brightest color.
    /// </summary>
    public double ColorRange { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps { get; set; }
}
```

#### GroupColorCalculator

```csharp
/// <summary>
/// Calculates the dominant color by grouping similar colors together.
/// </summary>
public class GroupColorCalculator
{
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    private int _brightness;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupColorCalculator"/> class with default settings.
    /// </summary>
    public GroupColorCalculator()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public GroupColorCalculator(double bigShift, double smallShift)
    
    /// <summary>
    /// Calculates the dominant color from the provided bitmap using color grouping.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated dominant color.</returns>
    public global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Gets or sets the color range to group colors.
    /// </summary>
    public double ColorRange { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum brightness threshold.
    /// </summary>
    public int Brightness { get; set; }
}
```

#### NearestColorCalculator

```csharp
/// <summary>
/// Calculates the nearest color based on difference logic.
/// </summary>
public class NearestColorCalculator
{
    private global::Avalonia.Media.Color _smallestDiff;
    private global::Avalonia.Media.Color _brightestColor;
    private double _colorRange;
    private double _bigShift;
    private double _smallShift;
    private int _pixelSteps;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NearestColorCalculator"/> class with default settings.
    /// </summary>
    public NearestColorCalculator()
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NearestColorCalculator"/> class with custom shift values.
    /// </summary>
    /// <param name="bigShift">The multiplier for dominant color components.</param>
    /// <param name="smallShift">The multiplier for non-dominant color components.</param>
    public NearestColorCalculator(double bigShift, double smallShift)
    
    /// <summary>
    /// Calculates the nearest color from the provided bitmap.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The calculated color.</returns>
    public unsafe global::Avalonia.Media.Color GetColorFromBitmap(Bitmap bitmap)
    
    /// <summary>
    /// Gets or sets the color with the smallest difference found.
    /// </summary>
    public global::Avalonia.Media.Color SmallestDiff { get; set; }
    
    /// <summary>
    /// Gets or sets the range within which colors are considered similar.
    /// </summary>
    public double ColorRange { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for dominant color components.
    /// </summary>
    public double BigShift { get; set; }
    
    /// <summary>
    /// Gets or sets the multiplier for non-dominant color components.
    /// </summary>
    public double SmallShift { get; set; }
    
    /// <summary>
    /// Gets or sets the step size for pixel sampling.
    /// </summary>
    public int PixelSteps { get; set; }
}
```

### Utils

#### ColorUtils

```csharp
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
}
```

## Data

### ClusterData

```csharp
/// <summary>
/// Contains static data for color clustering.
/// </summary>
public class ClusterData
{
    /// <summary>
    /// A pre-defined set of colors used for clustering or comparison.
    /// </summary>
    public static Color[] RGB_DATA
}
```
