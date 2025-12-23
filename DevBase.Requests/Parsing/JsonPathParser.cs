using System.Buffers;
using System.Text;
using System.Text.Json;

namespace DevBase.Requests.Parsing;

public sealed class JsonPathParser
{
    public T Parse<T>(ReadOnlySpan<byte> json, string path)
    {
        List<PathSegment> segments = ParsePath(path);
        using JsonDocument document = JsonDocument.Parse(json.ToArray());
        JsonElement? result = Navigate(document.RootElement, segments, 0);
        
        if (result == null)
            return default!;

        string jsonStr = result.Value.GetRawText();
        return JsonSerializer.Deserialize<T>(jsonStr)!;
    }

    public List<T> ParseList<T>(ReadOnlySpan<byte> json, string path)
    {
        List<PathSegment> segments = ParsePath(path);
        using JsonDocument document = JsonDocument.Parse(json.ToArray());
        List<JsonElement> results = NavigateAll(document.RootElement, segments, 0);
        
        return results
            .Select(r => JsonSerializer.Deserialize<T>(r.GetRawText())!)
            .ToList();
    }

    public async Task<T> ParseStreamAsync<T>(Stream stream, string path, CancellationToken cancellationToken = default)
    {
        List<PathSegment> segments = ParsePath(path);
        using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        JsonElement? result = Navigate(document.RootElement, segments, 0);

        if (result == null)
            return default!;

        string jsonStr = result.Value.GetRawText();
        return JsonSerializer.Deserialize<T>(jsonStr)!;
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
                {
                    segments.Add(new PathSegment { IsWildcard = true });
                }
                else if (int.TryParse(indexStr, out int index))
                {
                    segments.Add(new PathSegment { ArrayIndex = index });
                }
            }
            else
            {
                i++;
            }
        }

        return segments;
    }

    private JsonElement? Navigate(JsonElement element, List<PathSegment> segments, int segmentIndex)
    {
        if (segmentIndex >= segments.Count)
            return element;

        PathSegment segment = segments[segmentIndex];

        if (segment.IsRecursive)
        {
            return NavigateRecursive(element, segments, segmentIndex + 1);
        }

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

            List<JsonElement> results = new List<JsonElement>();
            foreach (JsonElement item in element.EnumerateArray())
            {
                JsonElement? result = Navigate(item, segments, segmentIndex + 1);
                if (result.HasValue)
                    results.Add(result.Value);
            }

            if (results.Count == 0)
                return null;
                
            return results[0];
        }

        return null;
    }

    private List<JsonElement> NavigateAll(JsonElement element, List<PathSegment> segments, int segmentIndex)
    {
        if (segmentIndex >= segments.Count)
            return [element];

        PathSegment segment = segments[segmentIndex];
        List<JsonElement> results = new List<JsonElement>();

        if (segment.IsRecursive)
        {
            results.AddRange(NavigateAllRecursive(element, segments, segmentIndex + 1));
            return results;
        }

        if (segment.PropertyName != null)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return results;

            if (!element.TryGetProperty(segment.PropertyName, out JsonElement prop))
                return results;

            return NavigateAll(prop, segments, segmentIndex + 1);
        }

        if (segment.ArrayIndex.HasValue)
        {
            if (element.ValueKind != JsonValueKind.Array)
                return results;

            int index = segment.ArrayIndex.Value;
            if (index >= 0 && index < element.GetArrayLength())
                results.AddRange(NavigateAll(element[index], segments, segmentIndex + 1));

            return results;
        }

        if (segment.IsWildcard)
        {
            if (element.ValueKind != JsonValueKind.Array)
                return results;

            foreach (JsonElement item in element.EnumerateArray())
            {
                results.AddRange(NavigateAll(item, segments, segmentIndex + 1));
            }
        }

        return results;
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
                JsonElement? result2 = NavigateRecursive(item, segments, segmentIndex);
                if (result.HasValue)
                    return result;
            }
        }

        return null;
    }

    private List<JsonElement> NavigateAllRecursive(JsonElement element, List<PathSegment> segments, int segmentIndex)
    {
        List<JsonElement> results = new List<JsonElement>();

        results.AddRange(NavigateAll(element, segments, segmentIndex));

        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty prop in element.EnumerateObject())
            {
                results.AddRange(NavigateAllRecursive(prop.Value, segments, segmentIndex));
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                results.AddRange(NavigateAllRecursive(item, segments, segmentIndex));
            }
        }

        return results;
    }
}
