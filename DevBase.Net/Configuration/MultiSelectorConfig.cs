namespace DevBase.Net.Configuration;

/// <summary>
/// Configuration for multiple selectors in scraping scenarios.
/// </summary>
public sealed class MultiSelectorConfig
{
    /// <summary>
    /// Gets the dictionary of selector names and paths.
    /// </summary>
    public Dictionary<string, string> Selectors { get; init; } = new();
    
    /// <summary>
    /// Gets a value indicating whether to optimize path reuse.
    /// </summary>
    public bool OptimizePathReuse { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether to optimize property access.
    /// </summary>
    public bool OptimizeProperties { get; init; }
    
    /// <summary>
    /// Gets the buffer size. Defaults to 4096.
    /// </summary>
    public int BufferSize { get; init; } = 4096;

    /// <summary>
    /// Creates a multi-selector configuration.
    /// </summary>
    /// <param name="selectors">Named selectors as (name, path) tuples.</param>
    /// <returns>A new <see cref="MultiSelectorConfig"/> instance.</returns>
    public static MultiSelectorConfig Create(params (string name, string path)[] selectors)
    {
        MultiSelectorConfig config = new MultiSelectorConfig
        {
            OptimizePathReuse = false,
            OptimizeProperties = false
        };
        
        foreach ((string name, string path) in selectors)
            config.Selectors[name] = path;
            
        return config;
    }
    
    /// <summary>
    /// Creates an optimized multi-selector configuration.
    /// </summary>
    /// <param name="selectors">Named selectors as (name, path) tuples.</param>
    /// <returns>A new <see cref="MultiSelectorConfig"/> instance with optimizations enabled.</returns>
    public static MultiSelectorConfig CreateOptimized(params (string name, string path)[] selectors)
    {
        MultiSelectorConfig config = new MultiSelectorConfig
        {
            OptimizePathReuse = true,
            OptimizeProperties = true
        };
        
        foreach ((string name, string path) in selectors)
            config.Selectors[name] = path;
            
        return config;
    }
}
