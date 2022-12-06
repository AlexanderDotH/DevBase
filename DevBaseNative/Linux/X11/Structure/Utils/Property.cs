using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using DevBaseNative.Linux.X11.Structure.Handle;

namespace DevBaseNative.Linux.X11.Structure.Utils;

public class Property
{
    
    public static string GetPropertyString(SafeHandle display, IntPtr win, string propName,
        ulong propType = (ulong) Enums.XAtom.XA_STRING)
    {
        using (var handle = GetProperty(display, win, propType, propName, out var size))
        {
            return GetString(handle, size);
        }
    }

    public static ulong GetPropertyNumber(SafeHandle display, IntPtr win, string propName)
    {
        using (var handle = GetProperty(display, win, Enums.XAtom.XA_CARDINAL, propName, out _))
        {
            return handle.IsInvalid ? default(ulong) : Marshal.PtrToStructure<ulong>(handle.DangerousGetHandle());
        }
    }
    
    public static string GetString(SafeHandle handle, ulong size)
    {
        return handle.IsInvalid ? default(string) : Marshal.PtrToStringAnsi(handle.DangerousGetHandle(), (int) size);
    }
    
    public static SafeHandle GetProperty(SafeHandle display, IntPtr win,
        Enums.XAtom xaPropType, string propName, out ulong size) =>
        GetProperty(display, win, (ulong) xaPropType, propName, out size);
    
    public static SafeHandle GetProperty(SafeHandle display, IntPtr win, ulong xaPropType, string propName,
        out ulong size)
    {
        size = 0;

        var xaPropName = Native.Native.XInternAtom(display, propName, false);

        int prop = Native.Native.XGetWindowProperty(display, win, xaPropName, 0,
            4096 / 4, false, xaPropType, out var actualTypeReturn, out var actualFormatReturn,
            out var nItemsReturn, out var bytesAfterReturn, out var propReturn);
            
        if (prop != 0)
        {
            return new XPropertyHandle(IntPtr.Zero, false);
        }

        if (actualTypeReturn != xaPropType)
        {
            return new XPropertyHandle(IntPtr.Zero, false);
        }

        size = nItemsReturn;
        return new XPropertyHandle(propReturn, false);
    }
}