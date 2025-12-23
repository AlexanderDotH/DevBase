using System.Text;

namespace DevBase.Requests.Parsing;

/// <summary>
/// Represents a segment in a JSON path expression.
/// </summary>
internal readonly struct PathSegment
{
    public string? PropertyName { get; init; }
    
    /// <summary>
    /// UTF8-encoded property name for zero-allocation comparison with Utf8JsonReader.ValueTextEquals()
    /// </summary>
    public byte[]? PropertyNameUtf8 { get; init; }
    
    public int? ArrayIndex { get; init; }
    public bool IsWildcard { get; init; }
    public bool IsRecursive { get; init; }

    public static PathSegment FromPropertyName(string name) => new PathSegment
    {
        PropertyName = name,
        PropertyNameUtf8 = Encoding.UTF8.GetBytes(name)
    };
}
