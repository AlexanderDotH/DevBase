namespace DevBase.Requests.Parsing;

/// <summary>
/// Represents a segment in a JSON path expression.
/// </summary>
internal readonly struct PathSegment
{
    public string? PropertyName { get; init; }
    public int? ArrayIndex { get; init; }
    public bool IsWildcard { get; init; }
    public bool IsRecursive { get; init; }
}
