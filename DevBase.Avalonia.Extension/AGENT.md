# DevBase.Avalonia.Extension - AI Agent Guide

This guide helps AI agents effectively use DevBase.Avalonia.Extension for advanced color analysis in Avalonia UI applications.

## Overview

DevBase.Avalonia.Extension provides advanced color analysis using LAB color space and sophisticated preprocessing capabilities.

**Target Framework:** .NET 9.0, Avalonia UI 11.x

## Core Components

### Color Calculators

```csharp
// RGB-based clustering
ClusterColorCalculator
List<Color> Calculate(Bitmap bitmap, int clusterCount)

// LAB-based clustering (more accurate)
LabClusterColorCalculator
List<Color> Calculate(Bitmap bitmap, int clusterCount)
```

### Converters

```csharp
RGBToLabConverter
(double L, double A, double B) Convert(Color rgb)
```

## Usage Patterns for AI Agents

### Pattern 1: LAB Color Clustering

```csharp
using DevBase.Avalonia.Extension.Color.Image;
using Avalonia.Media.Imaging;

var bitmap = new Bitmap("album-art.png");
var calculator = new LabClusterColorCalculator();

// Extract 5 dominant colors
var colors = calculator.Calculate(bitmap, clusterCount: 5);

// Use for UI theming
var primary = colors[0];
var secondary = colors[1];
var accent = colors[2];
```

### Pattern 2: Image Preprocessing

```csharp
using DevBase.Avalonia.Extension.Processing;
using DevBase.Avalonia.Extension.Configuration;

var config = new PreProcessingConfiguration
{
    Brightness = new BrightnessConfiguration
    {
        Enabled = true,
        MinBrightness = 0.3,
        MaxBrightness = 0.9
    },
    Chroma = new ChromaConfiguration
    {
        Enabled = true,
        MinChroma = 0.2
    },
    Filter = new FilterConfiguration
    {
        Enabled = true,
        ExcludeGrayscale = true
    }
};

var preprocessor = new ImagePreProcessor(config);
var processed = preprocessor.Process(bitmap);

// Now extract colors from processed image
var calculator = new LabClusterColorCalculator();
var colors = calculator.Calculate(processed, 5);
```

### Pattern 3: Music Player Theme

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Avalonia.Extension.Color.Image;
using DevBase.Avalonia.Extension.Configuration;
using DevBase.Avalonia.Extension.Processing;

public async Task<List<Color>> GetAlbumTheme(string trackId)
{
    // Get album art
    var deezer = new Deezer();
    var track = await deezer.GetSong(trackId);
    var bitmap = await DownloadBitmap(track.album.cover_xl);
    
    // Preprocess to remove dark/grayscale colors
    var config = new PreProcessingConfiguration
    {
        Brightness = new BrightnessConfiguration 
        { 
            Enabled = true, 
            MinBrightness = 0.4 
        },
        Chroma = new ChromaConfiguration 
        { 
            Enabled = true, 
            MinChroma = 0.3 
        },
        Filter = new FilterConfiguration
        {
            Enabled = true,
            ExcludeGrayscale = true
        }
    };
    
    var preprocessor = new ImagePreProcessor(config);
    var processed = preprocessor.Process(bitmap);
    
    // Extract colors using LAB
    var calculator = new LabClusterColorCalculator();
    return calculator.Calculate(processed, 5).ToList();
}
```

### Pattern 4: RGB to LAB Conversion

```csharp
using DevBase.Avalonia.Extension.Converter;
using Avalonia.Media;

var converter = new RGBToLabConverter();
var color = Colors.Red;

var (l, a, b) = converter.Convert(color);
Console.WriteLine($"L: {l}, A: {a}, B: {b}");

// L = Lightness (0-100)
// A = Green-Red axis
// B = Blue-Yellow axis
```

### Pattern 5: Filter Specific Colors

```csharp
var config = new PreProcessingConfiguration
{
    Filter = new FilterConfiguration
    {
        Enabled = true,
        ExcludeGrayscale = true,
        ExcludeColors = new List<Color> 
        { 
            Colors.White, 
            Colors.Black 
        }
    }
};

var preprocessor = new ImagePreProcessor(config);
var filtered = preprocessor.Process(bitmap);
```

## Important Concepts

### 1. LAB vs RGB Clustering

**Use LAB when:**
- Accuracy is important
- Perceptual color distance matters
- Creating color palettes for UI

**Use RGB when:**
- Speed is critical
- Simple color extraction
- Quick prototyping

```csharp
// RGB - faster but less accurate
var rgbCalc = new ClusterColorCalculator();
var rgbColors = rgbCalc.Calculate(bitmap, 5);

// LAB - slower but more accurate
var labCalc = new LabClusterColorCalculator();
var labColors = labCalc.Calculate(bitmap, 5);
```

### 2. Preprocessing Configuration

```csharp
// Minimal preprocessing
var minConfig = new PreProcessingConfiguration
{
    Brightness = new BrightnessConfiguration { Enabled = true, MinBrightness = 0.2 }
};

// Aggressive preprocessing
var maxConfig = new PreProcessingConfiguration
{
    Brightness = new BrightnessConfiguration 
    { 
        Enabled = true, 
        MinBrightness = 0.4,
        MaxBrightness = 0.85
    },
    Chroma = new ChromaConfiguration 
    { 
        Enabled = true, 
        MinChroma = 0.3 
    },
    Filter = new FilterConfiguration
    {
        Enabled = true,
        ExcludeGrayscale = true
    }
};
```

### 3. Cluster Count Selection

```csharp
// Simple palette (2-3 colors)
var simple = calculator.Calculate(bitmap, 3);

// Balanced palette (5-7 colors)
var balanced = calculator.Calculate(bitmap, 5);

// Detailed palette (10+ colors)
var detailed = calculator.Calculate(bitmap, 10);
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Too Many Clusters

```csharp
// Wrong - too many clusters for small image
var colors = calculator.Calculate(smallBitmap, 50); // Overkill!

// Correct
var colors = calculator.Calculate(smallBitmap, 5);
```

### ❌ Mistake 2: No Preprocessing

```csharp
// Wrong - extracting from raw image with dark/grayscale colors
var colors = calculator.Calculate(bitmap, 5);

// Correct - preprocess first
var config = new PreProcessingConfiguration
{
    Brightness = new BrightnessConfiguration { Enabled = true, MinBrightness = 0.3 },
    Filter = new FilterConfiguration { Enabled = true, ExcludeGrayscale = true }
};
var preprocessor = new ImagePreProcessor(config);
var processed = preprocessor.Process(bitmap);
var colors = calculator.Calculate(processed, 5);
```

### ❌ Mistake 3: Wrong Calculator for Use Case

```csharp
// Wrong - using RGB for accurate color palette
var calculator = new ClusterColorCalculator();
var colors = calculator.Calculate(bitmap, 5); // Less accurate

// Correct - use LAB for accurate palettes
var calculator = new LabClusterColorCalculator();
var colors = calculator.Calculate(bitmap, 5);
```

### ❌ Mistake 4: Not Scaling Large Images

```csharp
// Wrong - processing 4K image directly
var bitmap = new Bitmap("4k-image.png");
var colors = calculator.Calculate(bitmap, 5); // Slow!

// Correct - scale down first
var original = new Bitmap("4k-image.png");
var scaled = original.CreateScaledBitmap(new PixelSize(400, 400));
var colors = calculator.Calculate(scaled, 5);
```

## Configuration Reference

### BrightnessConfiguration

| Property | Type | Range | Description |
|----------|------|-------|-------------|
| Enabled | bool | - | Enable brightness filtering |
| MinBrightness | double | 0.0-1.0 | Minimum brightness threshold |
| MaxBrightness | double | 0.0-1.0 | Maximum brightness threshold |

### ChromaConfiguration

| Property | Type | Range | Description |
|----------|------|-------|-------------|
| Enabled | bool | - | Enable chroma filtering |
| MinChroma | double | 0.0-1.0 | Minimum chroma/saturation |

### FilterConfiguration

| Property | Type | Description |
|----------|------|-------------|
| Enabled | bool | Enable color filtering |
| ExcludeGrayscale | bool | Remove grayscale colors |
| ExcludeColors | List<Color> | Specific colors to exclude |

## Performance Tips

1. **Scale images** before processing (300-500px is usually sufficient)
2. **Use RGB** for quick analysis, **LAB** for accuracy
3. **Limit clusters** to 5-10 for most use cases
4. **Cache results** for frequently analyzed images
5. **Preprocess once** then extract multiple palettes

## Integration Examples

### With DevBase.Api

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Avalonia.Extension.Color.Image;

var deezer = new Deezer();
var track = await deezer.GetSong("123456");
var bitmap = await DownloadBitmap(track.album.cover_xl);

var calculator = new LabClusterColorCalculator();
var colors = calculator.Calculate(bitmap, 5);
```

### With DevBase.Avalonia

```csharp
using DevBase.Avalonia.Color.Image;
using DevBase.Avalonia.Extension.Color.Image;

// Basic analysis
var basic = new BrightestColorCalculator().Calculate(bitmap);

// Advanced analysis
var advanced = new LabClusterColorCalculator().Calculate(bitmap, 5);
```

## Quick Reference

| Task | Code |
|------|------|
| LAB clustering | `new LabClusterColorCalculator().Calculate(bitmap, count)` |
| RGB clustering | `new ClusterColorCalculator().Calculate(bitmap, count)` |
| RGB to LAB | `new RGBToLabConverter().Convert(color)` |
| Preprocess | `new ImagePreProcessor(config).Process(bitmap)` |
| Filter brightness | `BrightnessConfiguration { MinBrightness = 0.3 }` |
| Filter chroma | `ChromaConfiguration { MinChroma = 0.2 }` |
| Exclude grayscale | `FilterConfiguration { ExcludeGrayscale = true }` |

## Testing Considerations

- Test with various image types (photos, graphics, logos)
- Test with different cluster counts
- Test preprocessing configurations
- Verify LAB conversion accuracy
- Test performance with large images

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

- Avalonia
- DevBase
- DevBase.Avalonia
