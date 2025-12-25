using DevBase.Generics;

namespace DevBase.Extensions;

/// <summary>
/// Provides extension methods for AList.
/// </summary>
public static class AListExtension
{
    /// <summary>
    /// Converts an array to an AList.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="list">The array to convert.</param>
    /// <returns>An AList containing the elements of the array.</returns>
    public static AList<T> ToAList<T>(this T[] list) => new AList<T>(list);
}