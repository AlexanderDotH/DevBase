using System.Drawing.Imaging;
using Avalonia.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;

namespace DevBase.Avalonia.Color.Extensions;

public static class BitmapExtension
{
    public static Bitmap ToBitmap(this global::Avalonia.Media.Imaging.Bitmap bitmap)
    {
        using MemoryStream stream = new MemoryStream();
        bitmap.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return new Bitmap(stream);
    }

    public static global::Avalonia.Media.Imaging.Bitmap ToBitmap(this Bitmap bitmap)
    {
        using MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new global::Avalonia.Media.Imaging.Bitmap(memoryStream);
    }
    
    public static global::Avalonia.Media.Imaging.Bitmap ToBitmap(this SixLabors.ImageSharp.Image image)
    {
        using MemoryStream memoryStream = new MemoryStream();
        image.SaveAsBmp(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new global::Avalonia.Media.Imaging.Bitmap(memoryStream);
    }

    public static SixLabors.ImageSharp.Image ToImage(this global::Avalonia.Media.Imaging.Bitmap bitmap)
    {
        using MemoryStream memoryStream = new MemoryStream();
        bitmap.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return SixLabors.ImageSharp.Image.Load(memoryStream);
    } 
}