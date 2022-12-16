﻿using Avalonia.Platform;

namespace DevBase.Color.Extensions;

public static class LockedFramebufferExtensions
{
    public static Span<byte> GetPixels(this ILockedFramebuffer framebuffer)
    {
        unsafe
        {
            return new Span<byte>((byte*)framebuffer.Address, framebuffer.RowBytes * framebuffer.Size.Height);
        }
    }

    public static Span<byte> GetPixel(this ILockedFramebuffer framebuffer, int x, int y)
    {
        unsafe
        {
            var bytesPerPixel = framebuffer.Format.GetBytesPerPixel();
            var zero = (byte*)framebuffer.Address;
            var offset = framebuffer.RowBytes * y + bytesPerPixel * x;
            return new Span<byte>(zero + offset, bytesPerPixel);
        }
    }

    public static void SetPixel(this ILockedFramebuffer framebuffer, int x, int y, Avalonia.Media.Color color)
    {
        var pixel = framebuffer.GetPixel(x, y);

        var alpha = color.A / 255.0;

        switch (framebuffer.Format)
        {
            case PixelFormat.Rgb565:
                var value = (((color.R & 0b11111000) << 8) + ((color.G & 0b11111100) << 3) + (color.B >> 3));
                pixel[0] = (byte)value;
                pixel[1] = (byte)(value >> 8);
                break;

            case PixelFormat.Rgba8888:
                pixel[0] = (byte)(color.R * alpha);
                pixel[1] = (byte)(color.G * alpha);
                pixel[2] = (byte)(color.B * alpha);
                pixel[3] = color.A;
                break;

            case PixelFormat.Bgra8888:
                pixel[0] = (byte)(color.B * alpha);
                pixel[1] = (byte)(color.G * alpha);
                pixel[2] = (byte)(color.R * alpha);
                pixel[3] = color.A;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}