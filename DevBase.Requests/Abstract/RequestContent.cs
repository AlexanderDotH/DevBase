using System.Text;

namespace DevBase.Requests.Abstract;

public abstract class RequestContent
{
    public abstract bool IsValid(ReadOnlySpan<byte> content);
}