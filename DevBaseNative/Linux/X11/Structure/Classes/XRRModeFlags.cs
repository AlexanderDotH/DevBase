using System.Runtime.InteropServices;

namespace DevBaseNative.Linux.X11.Structure.Classes;

[StructLayout(LayoutKind.Sequential)]

public struct XRRModeFlags
{

    public ulong ModeFlags;
}