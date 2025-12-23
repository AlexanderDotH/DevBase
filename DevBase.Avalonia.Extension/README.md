# DevBase.Avalonia.Extension

**DevBase.Avalonia.Extension** provides advanced color analysis and image processing capabilities for Avalonia UI applications, including LAB color space conversion, clustering algorithms, and sophisticated image preprocessing.

## Features

- **LAB Color Space** - Perceptually uniform color analysis
- **K-Means Clustering** - Advanced color clustering algorithms
- **Image Preprocessing** - Brightness, chroma, and filter adjustments
- **Post-Processing** - Color refinement and normalization
- **Bitmap Extensions** - Enhanced bitmap manipulation methods

## Installation

```bash
dotnet add package DevBase.Avalonia.Extension
```

## Core Components

### Color Calculators

- **`ClusterColorCalculator`** - RGB-based K-means clustering
- **`LabClusterColorCalculator`** - LAB color space clustering (more accurate)

### Converters

- **`RGBToLabConverter`** - Convert RGB to LAB color space

### Configuration

- **`BrightnessConfiguration`** - Brightness adjustment settings
- **`ChromaConfiguration`** - Chroma/saturation settings
- **`FilterConfiguration`** - Color filtering options
- **`PreProcessingConfiguration`** - Combined preprocessing settings
- **`PostProcessingConfiguration`** - Post-processing options

### Processing

- **`ImagePreProcessor`** - Apply preprocessing to images

## Quick Start

### LAB Color Space Clustering

```csharp
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Extension.Color.Image;

var bitmap = new Bitmap("image.png");
var calculator = new LabClusterColorCalculator();

// Get 5 dominant colors using LAB color space
var dominantColors = calculator.Calculate(bitmap, clusterCount: 5);

foreach (var color in dominantColors)
{
    Console.WriteLine($"Color: {color}");
}
```

### RGB Clustering

```csharp
using DevBase.Avalonia.Extension.Color.Image;

var calculator = new ClusterColorCalculator();
var colors = calculator.Calculate(bitmap, clusterCount: 3);
```

### Image Preprocessing

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
var processedBitmap = preprocessor.Process(bitmap);
```

## Usage Examples

### Extract Color Palette with LAB

```csharp
using DevBase.Avalonia.Extension.Color.Image;
using Avalonia.Media;

public class ColorPaletteExtractor
{
    public List<Color> ExtractPalette(Bitmap image, int colorCount)
    {
        var calculator = new LabClusterColorCalculator();
        var colors = calculator.Calculate(image, colorCount);
        
        return colors.ToList();
    }
}
```

### Adaptive Theme Generation

```csharp
using DevBase.Avalonia.Extension.Color.Image;
using DevBase.Avalonia.Extension.Configuration;
using DevBase.Avalonia.Extension.Processing;

public class ThemeGenerator
{
    public (Color Primary, Color Secondary, Color Accent) GenerateTheme(Bitmap image)
    {
        // Preprocess image
        var config = new PreProcessingConfiguration
        {
            Brightness = new BrightnessConfiguration { Enabled = true, MinBrightness = 0.4 },
            Chroma = new ChromaConfiguration { Enabled = true, MinChroma = 0.3 }
        };
        
        var preprocessor = new ImagePreProcessor(config);
        var processed = preprocessor.Process(image);
        
        // Extract colors using LAB clustering
        var calculator = new LabClusterColorCalculator();
        var colors = calculator.Calculate(processed, clusterCount: 3);
        
        return (colors[0], colors[1], colors[2]);
    }
}
```

### Music Player with Advanced Color Analysis

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Avalonia.Extension.Color.Image;
using DevBase.Avalonia.Extension.Configuration;
using DevBase.Avalonia.Extension.Processing;

public class MusicPlayerTheme
{
    public async Task<List<Color>> GetAlbumColors(string trackId)
    {
        // Get album art
        var deezer = new Deezer();
        var track = await deezer.GetSong(trackId);
        var bitmap = await DownloadBitmap(track.album.cover_xl);
        
        // Preprocess
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
                MinChroma = 0.25 
            },
            Filter = new FilterConfiguration
            {
                Enabled = true,
                ExcludeGrayscale = true
            }
        };
        
        var preprocessor = new ImagePreProcessor(config);
        var processed = preprocessor.Process(bitmap);
        
        // Extract colors
        var calculator = new LabClusterColorCalculator();
        return calculator.Calculate(processed, clusterCount: 5).ToList();
    }
}
```

### Color Filtering

```csharp
public class ColorFilter
{
    public Bitmap FilterDarkColors(Bitmap image)
    {
        var config = new PreProcessingConfiguration
        {
            Brightness = new BrightnessConfiguration
            {
                Enabled = true,
                MinBrightness = 0.5  // Remove dark colors
            }
        };
        
        var preprocessor = new ImagePreProcessor(config);
        return preprocessor.Process(image);
    }
    
    public Bitmap FilterGrayscale(Bitmap image)
    {
        var config = new PreProcessingConfiguration
        {
            Filter = new FilterConfiguration
            {
                Enabled = true,
                ExcludeGrayscale = true  // Remove grayscale colors
            }
        };
        
        var preprocessor = new ImagePreProcessor(config);
        return preprocessor.Process(image);
    }
}
```

## Configuration Options

### BrightnessConfiguration

```csharp
public class BrightnessConfiguration
{
    public bool Enabled { get; set; }
    public double MinBrightness { get; set; }  // 0.0 - 1.0
    public double MaxBrightness { get; set; }  // 0.0 - 1.0
}
```

### ChromaConfiguration

```csharp
public class ChromaConfiguration
{
    public bool Enabled { get; set; }
    public double MinChroma { get; set; }  // 0.0 - 1.0
}
```

### FilterConfiguration

```csharp
public class FilterConfiguration
{
    public bool Enabled { get; set; }
    public bool ExcludeGrayscale { get; set; }
    public List<Color> ExcludeColors { get; set; }
}
```

## LAB Color Space

LAB color space is perceptually uniform, meaning the distance between colors corresponds to human perception:

- **L** - Lightness (0-100)
- **A** - Green-Red axis
- **B** - Blue-Yellow axis

### Why Use LAB?

- More accurate color clustering
- Better perceptual color distance
- Superior to RGB for color analysis

### RGB to LAB Conversion

```csharp
using DevBase.Avalonia.Extension.Converter;
using Avalonia.Media;

var converter = new RGBToLabConverter();
var rgbColor = Colors.Red;

var (l, a, b) = converter.Convert(rgbColor);
Console.WriteLine($"L: {l}, A: {a}, B: {b}");
```

## Advanced Usage

### Custom Clustering Parameters

```csharp
var calculator = new LabClusterColorCalculator();

// More clusters for detailed palette
var detailedPalette = calculator.Calculate(bitmap, clusterCount: 10);

// Fewer clusters for simplified palette
var simplePalette = calculator.Calculate(bitmap, clusterCount: 3);
```

### Combined Preprocessing

```csharp
var config = new PreProcessingConfiguration
{
    Brightness = new BrightnessConfiguration
    {
        Enabled = true,
        MinBrightness = 0.3,
        MaxBrightness = 0.85
    },
    Chroma = new ChromaConfiguration
    {
        Enabled = true,
        MinChroma = 0.2
    },
    Filter = new FilterConfiguration
    {
        Enabled = true,
        ExcludeGrayscale = true,
        ExcludeColors = new List<Color> { Colors.White, Colors.Black }
    }
};

var preprocessor = new ImagePreProcessor(config);
var result = preprocessor.Process(bitmap);
```

### Post-Processing

```csharp
var postConfig = new PostProcessingConfiguration
{
    NormalizeColors = true,
    RemoveDuplicates = true,
    SortByBrightness = true
};

// Apply post-processing to extracted colors
var processedColors = ApplyPostProcessing(colors, postConfig);
```

## Performance Considerations

- **LAB conversion** - Slightly slower than RGB but more accurate
- **Cluster count** - More clusters = longer processing time
- **Image size** - Scale down large images before processing
- **Preprocessing** - Each filter adds processing time

## Comparison: RGB vs LAB Clustering

| Aspect | RGB Clustering | LAB Clustering |
|--------|---------------|----------------|
| Speed | Faster | Slightly slower |
| Accuracy | Good | Excellent |
| Perceptual | No | Yes |
| Use Case | Quick analysis | Accurate color extraction |

## Integration with DevBase.Avalonia

Works seamlessly with DevBase.Avalonia:

```csharp
using DevBase.Avalonia.Color.Image;
using DevBase.Avalonia.Extension.Color.Image;

// Basic analysis (DevBase.Avalonia)
var basicCalculator = new BrightestColorCalculator();
var brightestColor = basicCalculator.Calculate(bitmap);

// Advanced analysis (DevBase.Avalonia.Extension)
var advancedCalculator = new LabClusterColorCalculator();
var palette = advancedCalculator.Calculate(bitmap, 5);
```

## Target Framework

- **.NET 9.0**
- **Avalonia UI 11.x**

## Dependencies

- **Avalonia** - UI framework
- **DevBase** - Core utilities
- **DevBase.Avalonia** - Base color analysis

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
