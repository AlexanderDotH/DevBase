# DevBase.Avalonia Agent Guide

## Overview
DevBase.Avalonia provides utilities for the Avalonia UI framework, specifically image color analysis.

## Key Classes

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `ColorCalculator` | `DevBase.Avalonia.Color` | Dominant/accent color extraction |
| `ColorConverter` | `DevBase.Avalonia.Color` | Color space conversions |

## Quick Reference

### Extract Dominant Color
```csharp
using DevBase.Avalonia.Color;
using Avalonia.Media.Imaging;

Bitmap image = new Bitmap("path/to/image.png");
ColorCalculator calculator = new ColorCalculator(image);

Color dominantColor = calculator.GetDominantColor();
Color accentColor = calculator.GetAccentColor();
```

### Color Conversion
```csharp
// RGB to HSL
var hsl = ColorConverter.RgbToHsl(255, 128, 64);

// HSL to RGB
var rgb = ColorConverter.HslToRgb(hsl.H, hsl.S, hsl.L);
```

## File Structure
```
DevBase.Avalonia/
└── Color/
    ├── ColorCalculator.cs
    └── ColorConverter.cs
```

## Important Notes

1. **Requires Avalonia** UI framework
2. **ColorCalculator** analyzes bitmap images
3. **Dominant color** = most prevalent color
4. **Accent color** = complementary/vibrant color
5. **Color space conversions** between RGB, HSL, HSV
