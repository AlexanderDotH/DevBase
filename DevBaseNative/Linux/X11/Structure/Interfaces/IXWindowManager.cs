using DevBase.Generic;
using DevBaseNative.Linux.X11.Structure.Classes;

namespace DevBaseNative.Linux.X11.Structure.Interfaces;

public interface IXWindowManager : IDisposable
{
    void Open(string displayName);
    void Close();
    bool TryGetXWindows(out GenericList<XWindowInfo> windows);
}