using System.Diagnostics;
using System.Runtime.InteropServices;
using DevBase.Typography;
using DevBaseNative.Linux.X11.Structure;
using DevBaseNative.Linux.X11.Structure.Classes;
using DevBaseNative.Linux.X11.Structure.Handle;
using DevBaseNative.Linux.X11.Structure.Native;
using DevBaseNative.Linux.X11.Structure.Utils;

namespace DevBaseNative.Linux.X11;

public class XWindowManager
{
    private SafeHandle _displayHandle;

    public XWindowManager(string displayName = null)
    {
        this._displayHandle = OpenDisplayHandle(displayName);
    }

    public void Test()
    {
        int nmonitors = 0;
        IntPtr p = Native.XRRGetMonitors(this._displayHandle, GetFocusedWindow().Id, true, out nmonitors);
        

        IntPtr ptr = Native.XRootWindow(this._displayHandle, 0);
        IntPtr n = Native.XRRGetScreenInfo(this._displayHandle, ptr);
        
        IntPtr resources = Native.XRRGetScreenResources(this._displayHandle, Native.XDefaultRootWindow(this._displayHandle));
        
        XRRScreenResources resource = (XRRScreenResources)Marshal.PtrToStructure(resources, typeof(XRRScreenResources));

        for (int i = 0; i < resource.nmode; i++)
        {
            IntPtr a = Marshal.ReadIntPtr(resource.modes, i * IntPtr.Size);
            if (a != IntPtr.Zero)
            {
                XRRModeInfo mode = Marshal.PtrToStructure<XRRModeInfo>(a);
                Console.WriteLine(mode.hTotal);
            }
        }
        //Console.WriteLine(resource.crtcs.xId.Id);
        // Console.WriteLine(resource.modes.dotClock);
        // Console.WriteLine(resource.modes.hTotal);
        // Console.WriteLine(resource.modes.vTotal);
        // ulong clock = resource.modes.dotClock;
        // uint hT = resource.modes.hTotal;
        // uint vT = resource.modes.vTotal;
        
        // if (clock > 0 && hT > 0 && vT > 0)
        // {
        //     ulong rate = resource.modes.dotClock / (resource.modes.hTotal * resource.modes.vTotal);
        //     Console.WriteLine(rate);
        // }

        //short rate = Native.XRRConfigCurrentRate(n);
        //Console.WriteLine(rate);
    }
    
    
    public XWindowInfo GetFocusedWindow()
    {
        SafeHandle handle = Property.GetProperty(
            this._displayHandle, 
            Native.XDefaultRootWindow(this._displayHandle), 
            DevBaseNative.Linux.X11.Structure.Enums.XAtom.XA_WINDOW,
            "_NET_ACTIVE_WINDOW", 
            out ulong size);

        if (handle.IsInvalid)
            return null;
            
        IntPtr handleToWindow = Marshal.ReadIntPtr(handle.DangerousGetHandle());
        return GetWindowInfo(this._displayHandle, handleToWindow);
    }
    
    private XWindowInfo GetWindowInfo(SafeHandle display, IntPtr handle)
    {
        WmClass wmClass = ParseWmClass(GetXWindowClass(display, handle));
        string windowTitle = GetWindowTitle(display, handle);
        ulong pid = GetPid(display, handle);
        string clientMachine = GetClientMachine(display, handle);
        
        Native.XGetGeometry(display, handle, out var junkRoot, out var junkX, out var junkY, out var width,
            out var height, out var borderWidth, out var depth);

        return new XWindowInfo
        {
            Id = handle,
            WmClass = wmClass,
            WmName = windowTitle,
            WmPid = pid,
            WmClientMachine = clientMachine,
            Geometry = new Geometry
            {
                X = junkX,
                Y = junkY,
                Width = width,
                Height = height,
                BorderWidth = borderWidth,
                Depth = depth
            }
        };
    }
    
    private string GetXWindowClass(SafeHandle display, IntPtr win) =>
        Property.GetPropertyString(display, win, "WM_CLASS") ?? string.Empty;

    private string GetClientMachine(SafeHandle display, IntPtr win) =>
        Property.GetPropertyString(display, win, "WM_CLIENT_MACHINE");
    
    private ulong GetPid(SafeHandle display, IntPtr win) => Property.GetPropertyNumber(display, win, "_NET_WM_PID");
    
    private string GetWindowTitle(SafeHandle display, IntPtr win)
    {
        string? netWmName = Property.GetPropertyString(display, win, "_NET_WM_NAME",
            Native.XInternAtom(display, "UTF8_STRING", false));
        string wmName = Property.GetPropertyString(display, win, "WM_NAME");
        return netWmName ?? wmName;
    }
    
    private WmClass ParseWmClass(string xWindowClass)
    {
        string[] classes = xWindowClass
            .Split('\0')
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .ToArray();
        
        string instance = classes.Length > 0 ? classes[0] : string.Empty;
        string @class = classes.Length > 1 ? classes[1] : string.Empty;

        return new WmClass {InstanceName = instance, ClassName = @class};
    }
    
    private SafeHandle OpenDisplayHandle(string displayName)
    {
        XDisplayHandle displayHandle = new XDisplayHandle(Native.XOpenDisplay(displayName), true);

        if (displayHandle.IsInvalid)
            throw new Exception("Invalid display handle");

        return displayHandle;
    }
}