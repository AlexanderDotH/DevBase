# DevBase.Avalonia

**DevBase.Avalonia** provides color analysis and image processing utilities for Avalonia UI applications. It specializes in extracting dominant colors, calculating color clusters, and analyzing image properties.

## Features

- **Color Extraction** - Extract dominant colors from images
- **Cluster Analysis** - Group similar colors using clustering algorithms
- **Brightness Calculation** - Find brightest colors in images
- **Color Utilities** - Color manipulation and conversion helpers
- **Avalonia Integration** - Works seamlessly with Avalonia UI bitmaps

## Installation

```bash
dotnet add package DevBase.Avalonia
```

## Core Components

### Color Calculators

- **`BrightestColorCalculator`** - Finds the brightest color in an image
- **`GroupColorCalculator`** - Groups similar colors together
- **`NearestColorCalculator`** - Finds nearest color to a target

### Extensions

- **`ColorExtension`** - Color manipulation methods
- **`ColorNormalizerExtension`** - Color normalization utilities
- **`LockedFramebufferExtensions`** - Direct pixel access helpers

### Utilities

- **`ColorUtils`** - General color utility methods

## Quick Start

### Extract Brightest Color

```csharp
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Image;

var bitmap = new Bitmap("image.png");
var calculator = new BrightestColorCalculator();

var brightestColor = calculator.Calculate(bitmap);
Console.WriteLine($"Brightest color: {brightestColor}");
```

### Group Similar Colors

```csharp
using DevBase.Avalonia.Color.Image;

var calculator = new GroupColorCalculator();
var colorGroups = calculator.Calculate(bitmap);

foreach (var group in colorGroups)
{
    Console.WriteLine($"Group: {group.Key}, Count: {group.Value}");
}
```

### Find Nearest Color

```csharp
using DevBase.Avalonia.Color.Image;
using Avalonia.Media;

var calculator = new NearestColorCalculator();
var targetColor = Colors.Blue;

var nearestColor = calculator.FindNearest(bitmap, targetColor);
Console.WriteLine($"Nearest to blue: {nearestColor}");
```

## Usage Examples

### Album Art Color Analysis

```csharp
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Image;

public class AlbumArtAnalyzer
{
    public Color GetDominantColor(string imagePath)
    {
        var bitmap = new Bitmap(imagePath);
        var calculator = new BrightestColorCalculator();
        
        return calculator.Calculate(bitmap);
    }
    
    public List<Color> GetColorPalette(string imagePath, int colorCount)
    {
        var bitmap = new Bitmap(imagePath);
        var calculator = new GroupColorCalculator();
        
        var groups = calculator.Calculate(bitmap);
        return groups.Keys.Take(colorCount).ToList();
    }
}
```

### UI Theme Generation

```csharp
public class ThemeGenerator
{
    public (Color Primary, Color Accent) GenerateTheme(Bitmap image)
    {
        var brightest = new BrightestColorCalculator().Calculate(image);
        var groups = new GroupColorCalculator().Calculate(image);
        
        var primary = brightest;
        var accent = groups.Keys.FirstOrDefault(c => c != primary);
        
        return (primary, accent);
    }
}
```

### Color Extensions

```csharp
using DevBase.Avalonia.Color.Extensions;
using Avalonia.Media;

var color = Colors.Red;

// Normalize color
var normalized = color.Normalize();

// Convert to different format
var converted = color.ToHsl();

// Adjust brightness
var brighter = color.AdjustBrightness(1.2);
```

## Integration with Avalonia UI

### Window Background Color

```csharp
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Image;

public class DynamicWindow : Window
{
    public void SetBackgroundFromImage(string imagePath)
    {
        var bitmap = new Bitmap(imagePath);
        var calculator = new BrightestColorCalculator();
        var dominantColor = calculator.Calculate(bitmap);
        
        this.Background = new SolidColorBrush(dominantColor);
    }
}
```

### Control Theming

```csharp
public class ThemedControl : UserControl
{
    public void ApplyImageTheme(Bitmap image)
    {
        var calculator = new GroupColorCalculator();
        var colors = calculator.Calculate(image);
        
        var primary = colors.Keys.First();
        var secondary = colors.Keys.Skip(1).First();
        
        // Apply to controls
        this.Foreground = new SolidColorBrush(primary);
        this.BorderBrush = new SolidColorBrush(secondary);
    }
}
```

## Data Structures

### ClusterData

```csharp
public class ClusterData
{
    public Color CenterColor { get; set; }
    public List<Color> Colors { get; set; }
    public int Count { get; set; }
}
```

## Performance Considerations

- **Image Size** - Larger images take longer to process
- **Color Depth** - More unique colors increase processing time
- **Caching** - Cache results for frequently analyzed images
- **Async Processing** - Consider async for large images

## Common Use Cases

### Use Case 1: Music Player UI

```csharp
public class MusicPlayer
{
    public void UpdateUIFromAlbumArt(Bitmap albumArt)
    {
        var calculator = new BrightestColorCalculator();
        var dominantColor = calculator.Calculate(albumArt);
        
        // Update player UI colors
        playerBackground.Background = new SolidColorBrush(dominantColor);
        progressBar.Foreground = new SolidColorBrush(dominantColor);
    }
}
```

### Use Case 2: Image Gallery

```csharp
public class ImageGallery
{
    public void GenerateThumbnailColors(List<string> imagePaths)
    {
        var calculator = new BrightestColorCalculator();
        
        foreach (var path in imagePaths)
        {
            var bitmap = new Bitmap(path);
            var color = calculator.Calculate(bitmap);
            
            // Store color for thumbnail border
            thumbnailColors[path] = color;
        }
    }
}
```

### Use Case 3: Adaptive UI

```csharp
public class AdaptiveUI
{
    public void AdaptToImage(Bitmap image)
    {
        var groups = new GroupColorCalculator().Calculate(image);
        var brightest = new BrightestColorCalculator().Calculate(image);
        
        // Determine if image is light or dark
        bool isLight = CalculateBrightness(brightest) > 0.5;
        
        // Adjust UI accordingly
        if (isLight)
        {
            textColor = Colors.Black;
            backgroundColor = Colors.White;
        }
        else
        {
            textColor = Colors.White;
            backgroundColor = Colors.Black;
        }
    }
}
```

## Target Framework

- **.NET 9.0**
- **Avalonia UI 11.x**

## Dependencies

- **Avalonia** - UI framework
- **Avalonia.Media** - Color and imaging
- **DevBase** - Core utilities

## License

MIT License - See LICENSE file for details

## Author

AlexanderDotH

## Repository

https://github.com/AlexanderDotH/DevBase
