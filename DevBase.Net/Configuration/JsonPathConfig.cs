namespace DevBase.Net.Configuration;

public sealed class JsonPathConfig
{
    public string? Path { get; init; }
    public bool StopAfterMatch { get; init; }
    public bool OptimizeArrays { get; init; } = false;
    public bool OptimizeProperties { get; init; } = false;
    public bool OptimizePathReuse { get; init; } = false;
    public int BufferSize { get; init; } = 4096;

    public static JsonPathConfig Create(string path, bool stopAfterMatch = false) => new()
    {
        Path = path,
        StopAfterMatch = stopAfterMatch,
        OptimizeArrays = false,
        OptimizeProperties = false,
        OptimizePathReuse = false
    };
    
    public static JsonPathConfig CreateOptimized(string path, bool stopAfterMatch = false) => new()
    {
        Path = path,
        StopAfterMatch = stopAfterMatch,
        OptimizeArrays = true,
        OptimizeProperties = true,
        OptimizePathReuse = true
    };
}
