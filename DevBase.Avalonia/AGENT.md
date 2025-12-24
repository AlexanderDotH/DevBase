# DevBase.Avalonia - AI Agent Guide

This guide helps AI agents effectively use DevBase.Avalonia for color analysis in Avalonia UI applications.

## Overview

DevBase.Avalonia provides color extraction and analysis tools for Avalonia UI bitmaps.

**Target Framework:** .NET 9.0, Avalonia UI 11.x  
**Current Version:** 1.0.0

---

## Project Structure

```
DevBase.Avalonia/
├── Color/
│   ├── Image/
│   │   ├── BrightestColorCalculator.cs   # Find brightest color
│   │   ├── GroupColorCalculator.cs       # Group similar colors
│   │   └── NearestColorCalculator.cs     # Find nearest color
│   └── ColorUtils.cs                     # Color utilities
└── Data/
    └── ...                               # Data structures
```

---

## Class Reference

### BrightestColorCalculator Class

**Namespace:** `DevBase.Avalonia.Color.Image`

Finds the brightest color in a bitmap.

#### Methods

```csharp
Color Calculate(Bitmap bitmap)
```

---

### GroupColorCalculator Class

**Namespace:** `DevBase.Avalonia.Color.Image`

Groups similar colors and counts occurrences.

#### Methods

```csharp
Dictionary<Color, int> Calculate(Bitmap bitmap)
```

---

### NearestColorCalculator Class

**Namespace:** `DevBase.Avalonia.Color.Image`

Finds the nearest color to a target.

#### Methods

```csharp
Color FindNearest(Bitmap bitmap, Color target)
```

---

## Core Components

### Color Calculators

```csharp
// Find brightest color
BrightestColorCalculator
Color Calculate(Bitmap bitmap)

// Group similar colors
GroupColorCalculator
Dictionary<Color, int> Calculate(Bitmap bitmap)

// Find nearest color
NearestColorCalculator
Color FindNearest(Bitmap bitmap, Color target)
```

## Usage Patterns for AI Agents

### Pattern 1: Extract Dominant Color

```csharp
using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Image;

var bitmap = new Bitmap("album-art.png");
var calculator = new BrightestColorCalculator();
var dominantColor = calculator.Calculate(bitmap);

// Use for UI theming
window.Background = new SolidColorBrush(dominantColor);
```

### Pattern 2: Color Palette Generation

```csharp
var calculator = new GroupColorCalculator();
var colorGroups = calculator.Calculate(bitmap);

// Get top 5 colors
var palette = colorGroups
    .OrderByDescending(g => g.Value)
    .Take(5)
    .Select(g => g.Key)
    .ToList();
```

### Pattern 3: Adaptive UI Theming

```csharp
public void ApplyImageTheme(Bitmap image)
{
    var brightest = new BrightestColorCalculator().Calculate(image);
    var groups = new GroupColorCalculator().Calculate(image);
    
    // Primary color
    var primary = brightest;
    
    // Secondary color (most common after primary)
    var secondary = groups
        .Where(g => g.Key != primary)
        .OrderByDescending(g => g.Value)
        .First()
        .Key;
    
    // Apply to UI
    ApplyColors(primary, secondary);
}
```

### Pattern 4: Music Player Integration

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Avalonia.Color.Image;

public async Task UpdatePlayerTheme(string trackId)
{
    // Get album art
    var deezer = new Deezer();
    var track = await deezer.GetSong(trackId);
    var albumArtUrl = track.album.cover_xl;
    
    // Download and analyze
    var bitmap = await DownloadBitmap(albumArtUrl);
    var calculator = new BrightestColorCalculator();
    var color = calculator.Calculate(bitmap);
    
    // Update player UI
    playerBackground.Background = new SolidColorBrush(color);
}
```

## Important Concepts

### 1. Bitmap Requirements

```csharp
// ✅ Correct - Avalonia bitmap
using Avalonia.Media.Imaging;
var bitmap = new Bitmap("image.png");

// ❌ Wrong - System.Drawing.Bitmap
using System.Drawing;
var bitmap = new Bitmap("image.png"); // Wrong type!
```

### 2. Color Analysis Performance

```csharp
// For large images, consider downscaling first
var originalBitmap = new Bitmap("large-image.png");
var scaledBitmap = originalBitmap.CreateScaledBitmap(
    new PixelSize(200, 200)
);

var calculator = new BrightestColorCalculator();
var color = calculator.Calculate(scaledBitmap);
```

### 3. Caching Results

```csharp
private Dictionary<string, Color> _colorCache = new();

public Color GetDominantColor(string imagePath)
{
    if (_colorCache.ContainsKey(imagePath))
        return _colorCache[imagePath];
    
    var bitmap = new Bitmap(imagePath);
    var calculator = new BrightestColorCalculator();
    var color = calculator.Calculate(bitmap);
    
    _colorCache[imagePath] = color;
    return color;
}
```

## Common Mistakes to Avoid

### ❌ Mistake 1: Wrong Bitmap Type

```csharp
// Wrong
using System.Drawing;
var bitmap = new Bitmap("image.png");
calculator.Calculate(bitmap); // Type mismatch!

// Correct
using Avalonia.Media.Imaging;
var bitmap = new Bitmap("image.png");
calculator.Calculate(bitmap);
```

### ❌ Mistake 2: Not Disposing Bitmaps

```csharp
// Wrong - memory leak
for (int i = 0; i < 1000; i++)
{
    var bitmap = new Bitmap($"image{i}.png");
    var color = calculator.Calculate(bitmap);
}

// Correct
for (int i = 0; i < 1000; i++)
{
    using var bitmap = new Bitmap($"image{i}.png");
    var color = calculator.Calculate(bitmap);
}
```

### ❌ Mistake 3: Processing Too Large Images

```csharp
// Wrong - slow for large images
var bitmap = new Bitmap("4k-image.png");
var color = calculator.Calculate(bitmap);

// Correct - scale down first
var original = new Bitmap("4k-image.png");
var scaled = original.CreateScaledBitmap(new PixelSize(300, 300));
var color = calculator.Calculate(scaled);
```

## Integration Examples

### With Music Player

```csharp
using DevBase.Api.Apis.Deezer;
using DevBase.Avalonia.Color.Image;
using Avalonia.Media.Imaging;

public class MusicPlayerViewModel
{
    private readonly BrightestColorCalculator _colorCalculator;
    
    public MusicPlayerViewModel()
    {
        _colorCalculator = new BrightestColorCalculator();
    }
    
    public async Task LoadTrack(string trackId)
    {
        var deezer = new Deezer();
        var track = await deezer.GetSong(trackId);
        
        // Load album art
        var bitmap = await LoadBitmapFromUrl(track.album.cover_xl);
        
        // Extract color
        var dominantColor = _colorCalculator.Calculate(bitmap);
        
        // Update UI
        BackgroundColor = dominantColor;
    }
}
```

### With Image Gallery

```csharp
public class ImageGalleryViewModel
{
    public async Task<List<(string Path, Color Color)>> AnalyzeImages(List<string> paths)
    {
        var calculator = new BrightestColorCalculator();
        var results = new List<(string, Color)>();
        
        foreach (var path in paths)
        {
            using var bitmap = new Bitmap(path);
            var color = calculator.Calculate(bitmap);
            results.Add((path, color));
        }
        
        return results;
    }
}
```

## Performance Tips

1. **Scale down large images** before analysis
2. **Cache results** for frequently accessed images
3. **Use async** for multiple image processing
4. **Dispose bitmaps** to free memory
5. **Limit color groups** when using GroupColorCalculator

## Quick Reference

| Task | Code |
|------|------|
| Load bitmap | `new Bitmap("path.png")` |
| Brightest color | `new BrightestColorCalculator().Calculate(bitmap)` |
| Group colors | `new GroupColorCalculator().Calculate(bitmap)` |
| Find nearest | `new NearestColorCalculator().FindNearest(bitmap, target)` |
| Apply to UI | `control.Background = new SolidColorBrush(color)` |

## Testing Considerations

- Test with various image sizes
- Test with different color depths
- Test with grayscale images
- Test with transparent images
- Verify memory disposal

## Version

Current version: **1.0.0**  
Target framework: **.NET 9.0**

## Dependencies

- Avalonia
- Avalonia.Media
- DevBase
