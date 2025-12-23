# DevBase.Avalonia

A utility library for Avalonia UI applications providing color manipulation, helper functions, and common UI utilities.

## Features

- **Color Utilities** - Color manipulation and conversion
- **Avalonia Helpers** - Common Avalonia UI helper functions
- **Cross-Platform** - Works on Windows, macOS, and Linux

## Installation

```xml
<PackageReference Include="DevBase.Avalonia" Version="x.x.x" />
```

Or via NuGet CLI:

```bash
dotnet add package DevBase.Avalonia
```

## Usage Examples

### Color Manipulation

```csharp
using DevBase.Avalonia.Color;

// Convert between color formats
var avaloniaColor = ColorUtils.FromHex("#FF5733");
var rgbColor = ColorUtils.ToRgb(avaloniaColor);

// Color blending
var blended = ColorUtils.Blend(color1, color2, 0.5);

// Brightness adjustment
var lighter = ColorUtils.Lighten(color, 0.2);
var darker = ColorUtils.Darken(color, 0.2);
```

### Color Schemes

```csharp
// Generate complementary colors
var complementary = ColorUtils.GetComplementary(baseColor);

// Generate color palette
var palette = ColorUtils.GeneratePalette(baseColor, 5);
```

## Architecture

```
DevBase.Avalonia/
├── Color/                # Color utilities
│   ├── ColorUtils.cs     # Color manipulation
│   ├── ColorConverter.cs # Format conversion
│   └── ...
└── Data/                 # Data helpers
```

## Dependencies

- Avalonia (11.x)
- System.Drawing.Common
- DevBase (core library)
- DevBase.Cryptography

## License

MIT License - see LICENSE file for details.
