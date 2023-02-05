using Avalonia.Controls;
using Avalonia.Input;

namespace DevBase.Avalonia.Scaling;

public abstract class ScalableWindow : Window
{
    public abstract event EventHandler<PointerPressedEventArgs> BeginResize;
    public abstract event EventHandler<PointerEventArgs> Resize;
    public abstract event EventHandler<PointerReleasedEventArgs> EndResize;
}