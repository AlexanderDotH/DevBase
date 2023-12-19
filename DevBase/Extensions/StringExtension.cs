using System.Text;

namespace DevBase.Extensions;

public static class StringExtension
{
    public static string Repeat(this string value, int amount)
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        for (int i = 1; i <= amount; i++)
            stringBuilder.Append(value);

        return stringBuilder.ToString();
    }
}