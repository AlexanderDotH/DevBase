using DevBase.Generics;

namespace DevBase.Extensions;

public static class AListExtension
{
    public static AList<T> ToAList<T>(this T[] list) => new AList<T>(list);
}