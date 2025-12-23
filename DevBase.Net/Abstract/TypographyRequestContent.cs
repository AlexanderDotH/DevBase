using System.Text;

namespace DevBase.Net.Abstract;

public abstract class TypographyRequestContent : RequestContent
{
    public Encoding Encoding { get; protected set; }
    
    protected TypographyRequestContent(Encoding encoding)
    {
        this.Encoding = encoding;
    }
}