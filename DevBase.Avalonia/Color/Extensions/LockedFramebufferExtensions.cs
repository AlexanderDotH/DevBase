﻿using Avalonia.Platform;

namespace DevBase.Avalonia.Color.Extensions;

public static class LockedFramebufferExtensions
{
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