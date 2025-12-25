using System.Text;

namespace DevBase.Extensions;

/// <summary>
/// Provides extension methods for strings.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Repeats a string a specified number of times.
    /// </summary>
    /// <param name="value">The string to repeat.</param>
    /// <param name="amount">The number of times to repeat.</param>
    /// <returns>The repeated string.</returns>
    public static string Repeat(this string value, int amount)
    {
        StringBuilder stringBuilder = new StringBuilder();
        
        for (int i = 1; i <= amount; i++)
            stringBuilder.Append(value);

        return stringBuilder.ToString();
    }
}