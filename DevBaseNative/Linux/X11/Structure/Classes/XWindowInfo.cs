namespace DevBaseNative.Linux.X11.Structure.Classes;

public class XWindowInfo
{
    public IntPtr Id { get; set; }
    public WmClass WmClass { get; set; }
    public string WmName { get; set; }
    public ulong WmPid { get; set; }
    public string WmClientMachine { get; set; }
    public Geometry Geometry { get; set; } = new Geometry();
}