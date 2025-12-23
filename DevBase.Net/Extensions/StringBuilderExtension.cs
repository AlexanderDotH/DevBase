using System.Text;

namespace DevBase.Net.Extensions;

public static class StringBuilderExtension
{
    public static void ToSpan(this StringBuilder stringBuilder, ref char[] converted)
    {
        char[] values = new char[stringBuilder.Length];
        Span<char> valueSpan = values;
        
        stringBuilder.CopyTo(0, valueSpan, stringBuilder.Length);

        converted = values;
    }
}