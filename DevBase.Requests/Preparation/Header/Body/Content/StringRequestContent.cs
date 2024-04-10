using System.Text;
using DevBase.Requests.Abstract;

namespace DevBase.Requests.Preparation.Header.Body.Content;

public class StringRequestContent : TypographyRequestContent 
{
    public StringRequestContent(Encoding encoding) : base(encoding) { }
    
    public override bool IsValid(ReadOnlySpan<byte> content)
    {
        if (content == null)
            return false;
        
        if (content.Length == 0)
            return false;

        return true;
    }
}