using System.Runtime.InteropServices;

namespace DevBaseNative.Linux.X11.Structure.Classes;

[StructLayout(LayoutKind.Sequential)]

public struct XRRModeInfo
{
     public RRMode id;
     public uint width;
     public uint height;
     public ulong dotClock;
     public uint hSyncStart;
     public uint hSyncEnd;
     public uint hTotal;
     public uint hSkew ;
     public uint vSyncStart;
     public uint vSyncEnd ;
    public uint vTotal;
     public uint name;
     public uint nameLength ;
    public XRRModeFlags modeFlags ;
}