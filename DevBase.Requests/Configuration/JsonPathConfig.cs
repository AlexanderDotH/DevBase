namespace DevBase.Requests.Configuration;

public sealed class JsonPathConfig
{
    public bool Enabled { get; init; }
    public string? Path { get; init; }
    public bool StopAfterMatch { get; init; }
    public bool OptimizeArrays { get; init; } = true;
    public bool OptimizeProperties { get; init; } = true;
    public int BufferSize { get; init; } = 4096;

    public static JsonPathConfig Create(string path, bool stopAfterMatch = false) => new()
    {
        Enabled = true,
        Path = path,
        StopAfterMatch = stopAfterMatch,
        OptimizeArrays = true,
        OptimizeProperties = true
    };
}
