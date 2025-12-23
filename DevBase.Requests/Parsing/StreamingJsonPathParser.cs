using System.Buffers;
using System.Text;
using System.Text.Json;

namespace DevBase.Requests.Parsing;

public sealed class StreamingJsonPathParser
{
    private readonly int _bufferSize;

    public StreamingJsonPathParser(int bufferSize = 4096)
    {
        _bufferSize = bufferSize;
    }

    /// <summary>
    /// Fast path for extracting primitive values (strings, numbers, bools) from arrays.
    /// Avoids MemoryStream and JsonSerializer overhead.
    /// </summary>
    public List<T> ParseAllFast<T>(ReadOnlySpan<byte> json, string path) where T : IConvertible
    {
        List<PathSegment> segments = ParsePath(path);
        List<T> results = new List<T>();
        
        // Pre-encode the target property name as UTF8 for fast comparison
        byte[]? targetPropertyUtf8 = null;
        foreach (PathSegment segment in segments)
        {
            if (segment.PropertyName != null)
            {
                targetPropertyUtf8 = Encoding.UTF8.GetBytes(segment.PropertyName);
                break;
            }
        }

        Utf8JsonReader reader = new Utf8JsonReader(json);
        int matchDepth = 0;
        int targetDepth = segments.Count;
        bool inTargetArray = false;
        bool expectValue = false;

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    if (matchDepth < targetDepth && segments[matchDepth].IsWildcard)
                    {
                        matchDepth++;
                        inTargetArray = true;
                    }
                    break;

                case JsonTokenType.EndArray:
                    if (inTargetArray && matchDepth > 0)
                    {
                        matchDepth--;
                        inTargetArray = false;
                    }
                    break;

                case JsonTokenType.StartObject:
                    break;

                case JsonTokenType.EndObject:
                    expectValue = false;
                    break;

                case JsonTokenType.PropertyName:
                    if (inTargetArray && targetPropertyUtf8 != null)
                    {
                        // Use ValueTextEquals for zero-allocation comparison
                        expectValue = reader.ValueTextEquals(targetPropertyUtf8);
                        if (!expectValue)
                        {
                            // Skip this property's value entirely
                            reader.Read();
                            reader.TrySkip();
                        }
                    }
                    break;

                case JsonTokenType.String:
                    if (expectValue)
                    {
                        if (typeof(T) == typeof(string))
                        {
                            results.Add((T)(object)reader.GetString()!);
                        }
                        expectValue = false;
                    }
                    break;

                case JsonTokenType.Number:
                    if (expectValue)
                    {
                        if (typeof(T) == typeof(int) && reader.TryGetInt32(out int intVal))
                            results.Add((T)(object)intVal);
                        else if (typeof(T) == typeof(long) && reader.TryGetInt64(out long longVal))
                            results.Add((T)(object)longVal);
                        else if (typeof(T) == typeof(double) && reader.TryGetDouble(out double doubleVal))
                            results.Add((T)(object)doubleVal);
                        else if (typeof(T) == typeof(decimal) && reader.TryGetDecimal(out decimal decVal))
                            results.Add((T)(object)decVal);
                        expectValue = false;
                    }
                    break;

                case JsonTokenType.True:
                case JsonTokenType.False:
                    if (expectValue && typeof(T) == typeof(bool))
                    {
                        results.Add((T)(object)(reader.TokenType == JsonTokenType.True));
                        expectValue = false;
                    }
                    break;
            }
        }

        return results;
    }

    public async IAsyncEnumerable<T> ParseStreamAsync<T>(
        Stream stream,
        string path,
        bool optimizeProperties = true,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<PathSegment> segments = ParsePath(path);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);

        try
        {
            JsonParserState state = new JsonParserState();
            MemoryStream resultBuffer = new MemoryStream();
            int bytesInBuffer = 0;

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(bytesInBuffer, _bufferSize - bytesInBuffer), cancellationToken);
                if (bytesRead == 0 && bytesInBuffer == 0)
                    break;

                bytesInBuffer += bytesRead;
                int consumed = ProcessBuffer(buffer.AsSpan(0, bytesInBuffer), ref state, segments, resultBuffer, optimizeProperties);

                if (state.HasCompleteResult)
                {
                    resultBuffer.Position = 0;
                    T? result = await JsonSerializer.DeserializeAsync<T>(resultBuffer, cancellationToken: cancellationToken);
                    if (result != null)
                        yield return result;

                    resultBuffer.SetLength(0);
                    state.HasCompleteResult = false;
                }

                if (consumed < bytesInBuffer)
                {
                    Buffer.BlockCopy(buffer, consumed, buffer, 0, bytesInBuffer - consumed);
                    bytesInBuffer -= consumed;
                }
                else
                {
                    bytesInBuffer = 0;
                }

                if (bytesRead == 0)
                    break;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public T ParseSingle<T>(ReadOnlySpan<byte> json, string path, bool optimizeProperties = true)
    {
        List<PathSegment> segments = ParsePath(path);
        JsonParserState state = new JsonParserState();
        MemoryStream resultBuffer = new MemoryStream();

        ProcessBuffer(json, ref state, segments, resultBuffer, optimizeProperties);

        if (resultBuffer.Length == 0)
            return default!;

        resultBuffer.Position = 0;
        return JsonSerializer.Deserialize<T>(resultBuffer.ToArray())!;
    }

    public List<T> ParseAll<T>(ReadOnlySpan<byte> json, string path, bool optimizeProperties = true)
    {
        List<PathSegment> segments = ParsePath(path);
        List<T> results = new List<T>();
        JsonParserState state = new JsonParserState();
        MemoryStream resultBuffer = new MemoryStream();
        int offset = 0;

        while (offset < json.Length)
        {
            int consumed = ProcessBuffer(json[offset..], ref state, segments, resultBuffer, optimizeProperties);
            offset += consumed;

            if (state.HasCompleteResult)
            {
                resultBuffer.Position = 0;
                T? result = JsonSerializer.Deserialize<T>(resultBuffer.ToArray());
                if (result != null)
                    results.Add(result);

                resultBuffer.SetLength(0);
                state.HasCompleteResult = false;
            }

            if (consumed == 0)
                break;
        }

        return results;
    }

    private int ProcessBuffer(
        ReadOnlySpan<byte> buffer,
        ref JsonParserState state,
        List<PathSegment> segments,
        MemoryStream resultBuffer,
        bool optimizeProperties)
    {
        Utf8JsonReader reader = new Utf8JsonReader(buffer, isFinalBlock: false, state.JsonReaderState);
        int consumed = 0;

        try
        {
            while (reader.Read())
            {
                consumed = (int)reader.BytesConsumed;

                if (state.MatchDepth == segments.Count)
                {
                    WriteValue(ref reader, resultBuffer);
                    state.HasCompleteResult = true;
                    state.MatchDepth = 0;
                    break;
                }

                PathSegment? currentSegment = state.MatchDepth < segments.Count ? segments[state.MatchDepth] : null;

                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        state.Depth++;
                        break;

                    case JsonTokenType.EndObject:
                        state.Depth--;
                        if (state.MatchDepth > 0 && state.DepthAtMatch[state.MatchDepth - 1] == state.Depth)
                            state.MatchDepth--;
                        break;

                    case JsonTokenType.StartArray:
                        state.Depth++;
                        if (currentSegment?.IsWildcard == true || currentSegment?.ArrayIndex == 0)
                        {
                            state.DepthAtMatch[state.MatchDepth] = state.Depth;
                            state.MatchDepth++;
                            state.ArrayIndex = 0;
                        }
                        break;

                    case JsonTokenType.EndArray:
                        state.Depth--;
                        if (state.MatchDepth > 0 && state.DepthAtMatch[state.MatchDepth - 1] == state.Depth)
                            state.MatchDepth--;
                        state.ArrayIndex = 0;
                        break;

                    case JsonTokenType.PropertyName:
                        if (currentSegment?.PropertyNameUtf8 != null &&
                            reader.ValueTextEquals(currentSegment.Value.PropertyNameUtf8))
                        {
                            state.DepthAtMatch[state.MatchDepth] = state.Depth;
                            state.MatchDepth++;
                        }
                        else if (optimizeProperties && state.MatchDepth < segments.Count)
                        {
                            // Use TrySkip for faster skipping
                            if (reader.Read())
                                reader.TrySkip();
                            consumed = (int)reader.BytesConsumed;
                        }
                        break;
                }
            }
        }
        catch (JsonException)
        {
            // Incomplete JSON, need more data
        }

        state.JsonReaderState = reader.CurrentState;
        return consumed;
    }

    private static void WriteValue(ref Utf8JsonReader reader, MemoryStream output)
    {
        using Utf8JsonWriter writer = new Utf8JsonWriter(output);
        WriteCurrentValue(ref reader, writer);
        writer.Flush();
    }

    private static void WriteCurrentValue(ref Utf8JsonReader reader, Utf8JsonWriter writer)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                writer.WriteStartObject();
                int objectDepth = reader.CurrentDepth;
                while (reader.Read() && !(reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == objectDepth))
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        writer.WritePropertyName(reader.GetString()!);
                        reader.Read();
                        WriteCurrentValue(ref reader, writer);
                    }
                }
                writer.WriteEndObject();
                break;

            case JsonTokenType.StartArray:
                writer.WriteStartArray();
                int arrayDepth = reader.CurrentDepth;
                while (reader.Read() && !(reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth == arrayDepth))
                {
                    WriteCurrentValue(ref reader, writer);
                }
                writer.WriteEndArray();
                break;

            case JsonTokenType.String:
                writer.WriteStringValue(reader.GetString());
                break;

            case JsonTokenType.Number:
                if (reader.TryGetInt64(out long longValue))
                    writer.WriteNumberValue(longValue);
                else if (reader.TryGetDouble(out double doubleValue))
                    writer.WriteNumberValue(doubleValue);
                break;

            case JsonTokenType.True:
                writer.WriteBooleanValue(true);
                break;

            case JsonTokenType.False:
                writer.WriteBooleanValue(false);
                break;

            case JsonTokenType.Null:
                writer.WriteNullValue();
                break;
        }
    }

    private static void SkipValue(ref Utf8JsonReader reader)
    {
        if (!reader.Read())
            return;

        if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.StartArray)
        {
            int depth = reader.CurrentDepth;
            while (reader.Read())
            {
                if ((reader.TokenType == JsonTokenType.EndObject || reader.TokenType == JsonTokenType.EndArray)
                    && reader.CurrentDepth == depth)
                    break;
            }
        }
    }

    private static List<PathSegment> ParsePath(string path)
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

}
