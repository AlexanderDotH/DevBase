namespace DevBase.Net.Configuration;

public sealed class MultiSelectorConfig
{
    public Dictionary<string, string> Selectors { get; init; } = new();
    public bool OptimizePathReuse { get; init; }
    public bool OptimizeProperties { get; init; }
    public int BufferSize { get; init; } = 4096;

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
