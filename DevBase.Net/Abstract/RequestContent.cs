namespace DevBase.Net.Abstract;

public abstract class RequestContent
{
    public abstract bool IsValid(ReadOnlySpan<byte> content);
}