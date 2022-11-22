using System.Runtime.InteropServices;
using DevBaseNative.Linux.X11.Structure.Classes;

namespace DevBaseNative.Linux.X11.Structure.Native;

public class Native
{
    private const string X11 = "libX11.so.6";

    [DllImport(X11)]
    internal static extern IntPtr XOpenDisplay(string displayName);

    [DllImport(X11)]
    internal static extern void XCloseDisplay(SafeHandle display);

    
    [DllImport(X11)]
    internal static extern string XDisplayName(out string display);
    
    [DllImport(X11)]
    internal static extern ulong XInternAtom(SafeHandle display, string atomName, bool onlyIfExists);
    
    /// <summary>
    /// int XGetWindowProperty(display, w, property, long_offset, long_length, delete, req_type, 
    ///        actual_type_return, actual_format_return, nitems_return, bytes_after_return, 
    ///        prop_return)
    ///        Display *display;
    ///        Window w;
    ///        Atom property;
    ///        long long_offset, long_length;
    ///        Bool delete;
    ///        Atom req_type; 
    ///        Atom *actual_type_return;
    ///        int *actual_format_return;
    ///        unsigned long *nitems_return;
    ///        unsigned long *bytes_after_return;
    ///        unsigned char **prop_return;
    /// </summary>
    /// <param name="display"></param>
    /// <param name="window"></param>
    /// <param name="atom"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="delete"></param>
    /// <param name="reqType"></param>
    /// <param name="actualTypeReturn"></param>
    /// <param name="actualFormatReturn"></param>
    /// <param name="nItemsReturn"></param>
    /// <param name="bytesAfterReturn"></param>
    /// <param name="propReturn"></param>
    /// <returns></returns>
    [DllImport(X11)]
    internal static extern int XGetWindowProperty(
        SafeHandle display, 
        IntPtr window, 
        ulong atom, 
        long offset, 
        long length, 
        bool delete, 
        ulong reqType, 
        out ulong actualTypeReturn, 
        out int actualFormatReturn, 
        out ulong nItemsReturn, 
        out ulong bytesAfterReturn, 
        out IntPtr propReturn);

    [DllImport(X11)]
    internal static extern int XFree(SafeHandle data);

    [DllImport(X11)]
    internal static extern IntPtr XDefaultRootWindow(SafeHandle display);
    
    [DllImport(X11)]
    internal static extern IntPtr XRootWindow(SafeHandle display, int number);
    
    [DllImport("libXrandr.so")]
    internal static extern IntPtr XRRGetScreenInfo(SafeHandle display, IntPtr screen);
    
    [DllImport("libXrandr.so")]
    internal static extern IntPtr XRRGetMonitors(SafeHandle display, IntPtr window, bool active, out int nmonitors);
    
    [DllImport("libXrandr.so")]
    internal static extern IntPtr XRRGetScreenResources(SafeHandle display, IntPtr screen);
    
    [DllImport("libXrandr.so")]
    internal static extern short XRRConfigCurrentRate(IntPtr config);

    [DllImport(X11)]
    internal static extern int XGetGeometry(
        SafeHandle display, 
        IntPtr drawable, 
        out IntPtr rootReturn, 
        out int xReturn,
        out int yReturn, 
        out uint widthReturn, 
        out uint heightReturn, 
        out uint borderWidthReturn,
        out uint depthReturn);
}