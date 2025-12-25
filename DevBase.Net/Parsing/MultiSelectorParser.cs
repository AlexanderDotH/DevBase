using System.Text.Json;
using DevBase.Net.Configuration;

namespace DevBase.Net.Parsing;

public sealed class MultiSelectorParser
{
    public MultiSelectorResult Parse(ReadOnlySpan<byte> json, MultiSelectorConfig config)
    {
        MultiSelectorResult result = new MultiSelectorResult();
        
        if (config.Selectors.Count == 0)
            return result;
        
        using JsonDocument document = JsonDocument.Parse(json.ToArray());
        
        if (config.OptimizePathReuse)
            return ParseWithPathReuse(document.RootElement, config, result);
        
        return ParseIndividual(document.RootElement, config, result);
    }
    
    public MultiSelectorResult Parse(ReadOnlySpan<byte> json, params (string name, string path)[] selectors)
    {
        MultiSelectorConfig config = MultiSelectorConfig.Create(selectors);
        return Parse(json, config);
    }
    
    public MultiSelectorResult ParseOptimized(ReadOnlySpan<byte> json, params (string name, string path)[] selectors)
    {
        MultiSelectorConfig config = MultiSelectorConfig.CreateOptimized(selectors);
        return Parse(json, config);
    }

    private MultiSelectorResult ParseIndividual(JsonElement root, MultiSelectorConfig config, MultiSelectorResult result)
    {
        foreach (KeyValuePair<string, string> selector in config.Selectors)
        {
            List<PathSegment> segments = ParsePath(selector.Value);
            JsonElement? value = Navigate(root, segments, 0);
            result.Set(selector.Key, value);
        }
        
        return result;
    }
    
    private MultiSelectorResult ParseWithPathReuse(JsonElement root, MultiSelectorConfig config, MultiSelectorResult result)
    {
        Dictionary<string, List<PathSegment>> parsedPaths = new();
        foreach (KeyValuePair<string, string> selector in config.Selectors)
            parsedPaths[selector.Key] = ParsePath(selector.Value);
        
        List<SelectorGroup> groups = GroupByCommonPrefix(parsedPaths);
        
        foreach (SelectorGroup group in groups)
        {
            JsonElement? commonElement = Navigate(root, group.CommonPrefix, 0);
            
            if (!commonElement.HasValue)
            {
                foreach (string name in group.Selectors.Keys)
                    result.Set(name, null);
                continue;
            }
            
            foreach (KeyValuePair<string, List<PathSegment>> selector in group.Selectors)
            {
                JsonElement? value = Navigate(commonElement.Value, selector.Value, 0);
                result.Set(selector.Key, value);
            }
        }
        
        return result;
    }
    
    private List<SelectorGroup> GroupByCommonPrefix(Dictionary<string, List<PathSegment>> paths)
    {
        if (paths.Count == 0)
            return new List<SelectorGroup>();
        
        if (paths.Count == 1)
        {
            KeyValuePair<string, List<PathSegment>> single = paths.First();
            return new List<SelectorGroup>
            {
                new SelectorGroup
                {
                    CommonPrefix = new List<PathSegment>(),
                    Selectors = new Dictionary<string, List<PathSegment>> 
                    { 
                        { single.Key, single.Value } 
                    }
                }
            };
        }
        
        List<PathSegment> commonPrefix = FindCommonPrefix(paths.Values.ToList());
        
        if (commonPrefix.Count == 0)
        {
            return new List<SelectorGroup>
            {
                new SelectorGroup
                {
                    CommonPrefix = new List<PathSegment>(),
                    Selectors = paths
                }
            };
        }
        
        Dictionary<string, List<PathSegment>> remainingPaths = new();
        foreach (KeyValuePair<string, List<PathSegment>> path in paths)
        {
            List<PathSegment> remaining = path.Value.Skip(commonPrefix.Count).ToList();
            remainingPaths[path.Key] = remaining;
        }
        
        return new List<SelectorGroup>
        {
            new SelectorGroup
            {
                CommonPrefix = commonPrefix,
                Selectors = remainingPaths
            }
        };
    }
    
    private List<PathSegment> FindCommonPrefix(List<List<PathSegment>> paths)
    {
        if (paths.Count == 0)
            return new List<PathSegment>();
        
        List<PathSegment> first = paths[0];
        int minLength = paths.Min(p => p.Count);
        List<PathSegment> commonPrefix = new List<PathSegment>();
        
        for (int i = 0; i < minLength; i++)
        {
            PathSegment segment = first[i];
            bool allMatch = paths.All(p => SegmentsEqual(p[i], segment));
            
            if (!allMatch)
                break;
                
            commonPrefix.Add(segment);
        }
        
        return commonPrefix;
    }
    
    private bool SegmentsEqual(PathSegment a, PathSegment b)
    {
        if (a.PropertyName != b.PropertyName)
            return false;
        if (a.ArrayIndex != b.ArrayIndex)
            return false;
        if (a.IsWildcard != b.IsWildcard)
            return false;
        if (a.IsRecursive != b.IsRecursive)
            return false;
        return true;
    }

    private JsonElement? Navigate(JsonElement element, List<PathSegment> segments, int segmentIndex)
    {
        if (segmentIndex >= segments.Count)
            return element;

        PathSegment segment = segments[segmentIndex];

        if (segment.IsRecursive)
            return NavigateRecursive(element, segments, segmentIndex + 1);

        if (segment.PropertyName != null)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            if (!element.TryGetProperty(segment.PropertyName, out JsonElement prop))
                return null;

            return Navigate(prop, segments, segmentIndex + 1);
        }

        if (segment.ArrayIndex.HasValue)
        {
            if (element.ValueKind != JsonValueKind.Array)
                return null;

            int index = segment.ArrayIndex.Value;
            if (index < 0 || index >= element.GetArrayLength())
                return null;

            return Navigate(element[index], segments, segmentIndex + 1);
        }

        if (segment.IsWildcard)
        {
            if (element.ValueKind != JsonValueKind.Array)
                return null;

            foreach (JsonElement item in element.EnumerateArray())
            {
                JsonElement? result = Navigate(item, segments, segmentIndex + 1);
                if (result.HasValue)
                    return result;
            }
        }

        return null;
    }

    private JsonElement? NavigateRecursive(JsonElement element, List<PathSegment> segments, int segmentIndex)
    {
        JsonElement? result = Navigate(element, segments, segmentIndex);
        if (result.HasValue)
            return result;

        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty prop in element.EnumerateObject())
            {
                result = NavigateRecursive(prop.Value, segments, segmentIndex);
                if (result.HasValue)
                    return result;
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                result = NavigateRecursive(item, segments, segmentIndex);
                if (result.HasValue)
                    return result;
            }
        }

        return null;
    }

    private List<PathSegment> ParsePath(string path)
    {
        List<PathSegment> segments = new List<PathSegment>();
        ReadOnlySpan<char> span = path.AsSpan();
        int i = 0;

        if (span.Length > 0 && span[0] == '$')
            i++;

        while (i < span.Length)
        {
            if (span[i] == '.')
            {
                i++;
                
                if (i < span.Length && span[i] == '.')
                {
                    i++;
                    segments.Add(new PathSegment { IsRecursive = true });
                }

                int start = i;
                while (i < span.Length && span[i] != '.' && span[i] != '[')
                    i++;

                if (i > start)
                {
                    string propName = span[start..i].ToString();
                    segments.Add(PathSegment.FromPropertyName(propName));
                }
            }
            else if (span[i] == '[')
            {
                i++;
                int start = i;
                
                while (i < span.Length && span[i] != ']')
                    i++;

                string indexStr = span[start..i].ToString().Trim();
                i++;

                if (indexStr == "*")
                    segments.Add(new PathSegment { IsWildcard = true });
                else if (int.TryParse(indexStr, out int index))
                    segments.Add(new PathSegment { ArrayIndex = index });
            }
            else
            {
                i++;
            }
        }

        return segments;
    }
    
    private sealed class SelectorGroup
    {
        public List<PathSegment> CommonPrefix { get; init; } = new();
        public Dictionary<string, List<PathSegment>> Selectors { get; init; } = new();
    }
}
