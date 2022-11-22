namespace DevBaseNative.Linux.X11.Structure.Classes;

class XRRScreenConfiguration
{
    public ulong Screen { get; set; }
    public ulong Sizes { get; set; }
    public Object Rotations { get; set; } 
    public Object CurrentRotations { get; set; }
    public int NSizes { get; set; }
    public XRRScreenSize CurrentSize { get; set; }
    public short CurrentRate { get; set; }
    public Object TimeStamp { get; set; }
    public Object ConfigTimestamp { get; set; }
    public int SubpixelOrder { get; set; }
    public short Rates { get; set; }
    public int NRates { get; set; }
}