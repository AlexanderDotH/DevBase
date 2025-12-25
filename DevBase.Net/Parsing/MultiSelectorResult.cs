using System.Text.Json;

namespace DevBase.Net.Parsing;

public sealed class MultiSelectorResult
{
    private readonly Dictionary<string, string?> _results = new();
    
    public void Set(string name, JsonElement? value)
    {
        if (value.HasValue)
            _results[name] = value.Value.Clone().GetRawText();
        else
            _results[name] = null;
    }
    
    public bool HasValue(string name) => _results.TryGetValue(name, out string? value) && value != null;
    
    public T? Get<T>(string name)
    {
        if (!_results.TryGetValue(name, out string? json) || json == null)
            return default;
            
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
    
    public string? GetString(string name)
    {
        if (!_results.TryGetValue(name, out string? json) || json == null)
            return null;
        
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind == JsonValueKind.String 
                ? doc.RootElement.GetString() 
                : json;
        }
        catch
        {
            return json;
        }
    }
    
    public int? GetInt(string name)
    {
        if (!_results.TryGetValue(name, out string? json) || json == null)
            return null;
        
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetInt32(out int value) ? value : null;
        }
        catch
        {
            return null;
        }
    }
    
    public long? GetLong(string name)
    {
        if (!_results.TryGetValue(name, out string? json) || json == null)
            return null;
        
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetInt64(out long value) ? value : null;
        }
        catch
        {
            return null;
        }
    }
    
    public double? GetDouble(string name)
    {
        if (!_results.TryGetValue(name, out string? json) || json == null)
            return null;
        
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetDouble(out double value) ? value : null;
        }
        catch
        {
            return null;
        }
    }
    
    public bool? GetBool(string name)
    {
        if (!_results.TryGetValue(name, out string? json) || json == null)
            return null;
        
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return doc.RootElement.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }
    
    public IEnumerable<string> Names => _results.Keys;
    
    public int Count => _results.Count;
}
