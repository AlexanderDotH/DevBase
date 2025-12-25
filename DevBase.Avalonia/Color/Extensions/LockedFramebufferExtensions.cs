using Avalonia.Platform;

namespace DevBase.Avalonia.Color.Extensions;

/// <summary>
/// Provides extension methods for accessing pixel data from a <see cref="ILockedFramebuffer"/>.
/// </summary>
public static class LockedFramebufferExtensions
{
    /// <summary>
    /// Gets the pixel data at the specified coordinates as a span of bytes.
    /// </summary>
    /// <param name="framebuffer">The locked framebuffer.</param>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>A span of bytes representing the pixel.</returns>
    public static Span<byte> GetPixel(this ILockedFramebuffer framebuffer, int x, int y)
    {
        unsafe
        {
            var bytesPerPixel = framebuffer.Format.BitsPerPixel / 8;
            var zero = (byte*)framebuffer.Address;
            var offset = framebuffer.RowBytes * y + bytesPerPixel * x;
            return new Span<byte>(zero + offset, bytesPerPixel);
        }
    }
}