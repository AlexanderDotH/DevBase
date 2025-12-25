namespace DevBase.Net.Configuration;

/// <summary>
/// Configuration for JSON path extraction.
/// </summary>
public sealed class JsonPathConfig
{
    /// <summary>
    /// Gets the JSON path to extract.
    /// </summary>
    public string? Path { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether to stop after the first match.
    /// </summary>
    public bool StopAfterMatch { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether to optimize for arrays.
    /// </summary>
    public bool OptimizeArrays { get; init; } = false;
    
    /// <summary>
    /// Gets a value indicating whether to optimize for properties.
    /// </summary>
    public bool OptimizeProperties { get; init; } = false;
    
    /// <summary>
    /// Gets a value indicating whether to optimize path reuse.
    /// </summary>
    public bool OptimizePathReuse { get; init; } = false;
    
    /// <summary>
    /// Gets the buffer size for parsing. Defaults to 4096.
    /// </summary>
    public int BufferSize { get; init; } = 4096;

    /// <summary>
    /// Creates a basic JSON path configuration.
    /// </summary>
    /// <param name="path">The JSON path.</param>
    /// <param name="stopAfterMatch">Whether to stop after the first match.</param>
    /// <returns>A new <see cref="JsonPathConfig"/> instance.</returns>
    public static JsonPathConfig Create(string path, bool stopAfterMatch = false) => new()
    {
        Path = path,
        StopAfterMatch = stopAfterMatch,
        OptimizeArrays = false,
        OptimizeProperties = false,
        OptimizePathReuse = false
    };
    
    /// <summary>
    /// Creates an optimized JSON path configuration.
    /// </summary>
    /// <param name="path">The JSON path.</param>
    /// <param name="stopAfterMatch">Whether to stop after the first match.</param>
    /// <returns>A new <see cref="JsonPathConfig"/> instance with optimizations enabled.</returns>
    public static JsonPathConfig CreateOptimized(string path, bool stopAfterMatch = false) => new()
    {
        Path = path,
        StopAfterMatch = stopAfterMatch,
        OptimizeArrays = true,
        OptimizeProperties = true,
        OptimizePathReuse = true
    };
}
