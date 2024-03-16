using System.Text;

namespace DevBase.Requests.Abstract;

public abstract class TypographyRequestContent : RequestContent
{
    protected Encoding Encoding { get; set; }
    
    protected TypographyRequestContent(Encoding encoding)
    {
        this.Encoding = encoding;
    }
}