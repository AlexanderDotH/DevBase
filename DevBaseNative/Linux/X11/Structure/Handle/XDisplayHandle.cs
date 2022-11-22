using System.Runtime.InteropServices;

namespace DevBaseNative.Linux.X11.Structure.Handle;

internal class XDisplayHandle : SafeHandle
{
    private readonly IntPtr _handle;

    public XDisplayHandle(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
    {
        this._handle = invalidHandleValue;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ReleaseHandle();
        }

        base.Dispose(disposing);
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            Structure.Native.Native.XCloseDisplay(this);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public override bool IsInvalid => _handle == IntPtr.Zero;
}