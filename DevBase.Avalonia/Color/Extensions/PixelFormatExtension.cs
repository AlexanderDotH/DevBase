using Avalonia.Platform;

namespace DevBase.Avalonia.Color.Extensions;

public static class PixelFormatExtension
{
    public static int BitsPerPixel(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.Rgb565:
                return 16;
            case PixelFormat.Rgba8888:
            case PixelFormat.Bgra8888:
                return 32;
            default:
                return 8;
        }
    }
}