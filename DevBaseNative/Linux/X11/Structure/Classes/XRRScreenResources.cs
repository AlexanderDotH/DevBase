using System.Runtime.InteropServices;

namespace DevBaseNative.Linux.X11.Structure.Classes;

[StructLayout(LayoutKind.Sequential, Size = 7)]
public struct XRRScreenResources
{
    public Time timestamp;
     public Time configTimestamp;
    public int ncrtc;
     public RRCrtc crtcs;
     public int noutput;
     public RROutput outputs;
     public int nmode;
     public XRRModeInfo modes;
}