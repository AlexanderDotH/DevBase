using Colourful;

namespace DevBase.Avalonia.Color.Converter;

public class RGBToLabConverter
{
    private IColorConverter<RGBColor, LabColor> _converter;
    private IColorConverter<LabColor, RGBColor> _unconverter;

    
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

    public LabColor ToLabColor(RGBColor color)
    {
        return this._converter.Convert(color);
    }
    
    public RGBColor ToRgbColor(LabColor color)
    {
        return this._unconverter.Convert(color);
    }
}