using Avalonia.Media.Imaging;
using DevBase.Avalonia.Color.Extensions;
using DevBase.Avalonia.Extension.Extension;
using SixLabors.ImageSharp.Processing;
using Bitmap = System.Drawing.Bitmap;

namespace DevBase.Avalonia.Extension.Processing;

/// <summary>
/// Provides image pre-processing functionality, such as blurring.
/// </summary>
public class ImagePreProcessor
{
    private readonly float _sigma;
    private readonly int _rounds;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ImagePreProcessor"/> class.
    /// </summary>
    /// <param name="sigma">The Gaussian blur sigma value.</param>
    /// <param name="rounds">The number of blur iterations.</param>
    public ImagePreProcessor(float sigma, int rounds = 10)
    {
        this._sigma = sigma;
        this._rounds = rounds;
    }

    /// <summary>
    /// Processes an Avalonia Bitmap by applying Gaussian blur.
    /// </summary>
    /// <param name="bitmap">The source bitmap.</param>
    /// <returns>The processed bitmap.</returns>
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