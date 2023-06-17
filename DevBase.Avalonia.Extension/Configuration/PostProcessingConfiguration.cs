namespace DevBase.Avalonia.Extension.Configuration;

public class PostProcessingConfiguration
{
    public double SmallShift { get; set; }
    public double BigShift { get; set; }
    public bool ColorShiftingPostProcessing { get; set; }
    public double PastelLightness { get; set; }
    public double PastelLightnessSubtractor { get; set; }
    public double PastelSaturation { get; set; }
    public double PastelGuidance { get; set; }
    
    public bool PastelPostProcessing { get; set; }
}