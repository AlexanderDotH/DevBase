using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Extension.Extension;
using Bitmap = System.Drawing.Bitmap;

namespace DevBase.Avalonia.Extension.Processing;

public class ImagePreProcessor
{
    private readonly float _sigma;
    private readonly int _rounds;
    
    public ImagePreProcessor(float sigma, int rounds = 10)
    {
        this._sigma = sigma;
        this._rounds = rounds;
    }

    public global::Avalonia.Media.Imaging.Bitmap Process(global::Avalonia.Media.Imaging.Bitmap bitmap)
    {
        SixLabors.ImageSharp.Image img = bitmap.ToImage();
        
        for (int i = 0; i < this._rounds; i++)
        {
            img.Mutate(x => x.GaussianBlur(this._sigma));
        }

        return img.ToBitmap();
    }
}