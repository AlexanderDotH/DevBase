using Colourful;

namespace DevBase.Avalonia.Extension.Converter;

/// <summary>
/// Converter for transforming between RGB and LAB color spaces.
/// </summary>
public class RGBToLabConverter
{
    private IColorConverter<RGBColor, LabColor> _converter;
    private IColorConverter<LabColor, RGBColor> _unconverter;

    
    /// <summary>
    /// Initializes a new instance of the <see cref="RGBToLabConverter"/> class.
    /// Configures converters using sRGB working space and D65 illuminant.
    /// </summary>
    public RGBToLabConverter()
    {
        this._converter = new ConverterBuilder()
            .FromRGB(RGBWorkingSpaces.sRGB)
            .ToLab(Illuminants.D65)
            .Build();
        
        this._unconverter = new ConverterBuilder()
            .FromLab(Illuminants.D65)
            .ToRGB(RGBWorkingSpaces.sRGB)
            .Build();
    }

    /// <summary>
    /// Converts an RGB color to Lab color.
    /// </summary>
    /// <param name="color">The RGB color.</param>
    /// <returns>The Lab color.</returns>
    public LabColor ToLabColor(RGBColor color)
    {
        return this._converter.Convert(color);
    }
    
    /// <summary>
    /// Converts a Lab color to RGB color.
    /// </summary>
    /// <param name="color">The Lab color.</param>
    /// <returns>The RGB color.</returns>
    public RGBColor ToRgbColor(LabColor color)
    {
        return this._unconverter.Convert(color);
    }
}