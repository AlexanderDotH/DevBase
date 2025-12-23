using DevBase.Net.Abstract;

namespace DevBase.Net.Data.Body.Content;

public class BufferRequestContent : RequestContent
{
    public override bool IsValid(ReadOnlySpan<byte> content)
    {
        if (content == null)
            return false;
        
        if (content.IsEmpty)
            return false;
        
        return true;
    }
}