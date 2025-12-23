# DevBase.Avalonia & DevBase.Avalonia.Extension

These libraries provide powerful tools for working with colors and images within the Avalonia UI framework. They are particularly useful for extracting color palettes from images, performing color space conversions (RGB <-> Lab), and manipulating bitmaps.

## Table of Contents
- [DevBase.Avalonia](#devbaseavalonia)
  - [Image Color Calculators](#image-color-calculators)
- [DevBase.Avalonia.Extension](#devbaseavaloniaextension)
  - [Bitmap Extensions](#bitmap-extensions)
  - [LabColor Extensions](#labcolor-extensions)

---

## DevBase.Avalonia

Focuses on analyzing images to extract meaningful color data.

### Image Color Calculators
Located in `DevBase.Avalonia.Color.Image`, these classes help you extract specific types of colors from a `Bitmap`.

#### Common Parameters
Most calculators share these configuration properties:
- **PixelSteps**: Determines how many pixels to skip when sampling (higher = faster but less accurate). Default: 10.
- **ColorRange**: The tolerance for grouping similar colors.
- **BigShift / SmallShift**: Weights used during color averaging/correction.

#### BrightestColorCalculator
Finds the brightest color in an image. It identifies the pixel with the highest calculated brightness and then averages similar surrounding pixels to return a representative color.

```csharp
using DevBase.Avalonia.Color.Image;

var calculator = new BrightestColorCalculator();
var brightestColor = calculator.GetColorFromBitmap(myBitmap);
```

#### GroupColorCalculator
Groups similar colors together to find the "dominant" color group in an image. It handles noise by filtering out small groups and averaging the largest color cluster.

```csharp
var calculator = new GroupColorCalculator();
calculator.Brightness = 20; // Ignore very dark colors
var dominantColor = calculator.GetColorFromBitmap(myBitmap);
```

#### NearestColorCalculator
Finds colors that are distinct or closest to a baseline, useful for finding accent colors that stand out or blend in.

```csharp
var calculator = new NearestColorCalculator();
var accentColor = calculator.GetColorFromBitmap(myBitmap);
```

---

## DevBase.Avalonia.Extension

Provides extension methods for seamless interoperability and advanced color math.

### Bitmap Extensions
Facilitates conversion between different image libraries.
- **Avalonia** (`Avalonia.Media.Imaging.Bitmap`)
- **System.Drawing** (`System.Drawing.Bitmap`)
- **ImageSharp** (`SixLabors.ImageSharp.Image`)

**Example:**
```csharp
using DevBase.Avalonia.Extension.Extension;

// Avalonia -> System.Drawing
System.Drawing.Bitmap sysBitmap = avaloniaBitmap.ToBitmap();

// System.Drawing -> Avalonia
Avalonia.Media.Imaging.Bitmap avBitmap = sysBitmap.ToBitmap();

// ImageSharp -> Avalonia
Avalonia.Media.Imaging.Bitmap avBitmapFromSharp = imageSharpImage.ToBitmap();
```

### LabColor Extensions
Extends the `Colourful.LabColor` type for advanced filtering and manipulation.

**Features:**
- **FilterBrightness**: Returns colors within a specific lightness (L) range.
- **FilterChroma**: Returns colors within a specific chromatic intensity.
- **ToPastel**: Converts a color to a pastel version by adjusting lightness and saturation.
- **Converters**: Batch convert `AList<RGBColor>` to `AList<LabColor>` and vice versa.

**Example:**
```csharp
using DevBase.Avalonia.Extension.Extension;

// Filter for bright colors
var brightColors = allLabColors.FilterBrightness(min: 70, max: 100);

// Convert to pastel
var pastelColor = myLabColor.ToPastel();
```
