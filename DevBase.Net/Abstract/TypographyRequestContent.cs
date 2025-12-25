using System.Text;

namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for request content that deals with text encoding.
/// </summary>
public abstract class TypographyRequestContent : RequestContent
{
    /// <summary>
    /// Gets or sets the encoding used for the content.
    /// </summary>
    public Encoding Encoding { get; protected set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TypographyRequestContent"/> class.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    protected TypographyRequestContent(Encoding encoding)
    {
        this.Encoding = encoding;
    }
}