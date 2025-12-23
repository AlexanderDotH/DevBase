using System.Text;

namespace DevBase.Requests.Utils;

public static class StringBuilderPool
{
    [ThreadStatic]
    private static StringBuilder? _instance;

    public static StringBuilder Acquire(int capacity = 64)
    {
        StringBuilder sb = _instance ??= new StringBuilder(capacity);
        sb.Clear();
        
        if (sb.Capacity < capacity)
            sb.Capacity = capacity;
            
        return sb;
    }

    public static string ToStringAndRelease(this StringBuilder sb)
    {
        return sb.ToString();
    }

    public static StringBuilder Append(this StringBuilder sb, ReadOnlyMemory<char> value)
    {
        sb.Append(value.Span);
        return sb;
    }
}
