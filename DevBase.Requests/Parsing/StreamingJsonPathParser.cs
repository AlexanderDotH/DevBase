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

    public async IAsyncEnumerable<T> ParseStreamAsync<T>(
        Stream stream,
        string path,
        bool optimizeProperties = true,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var segments = ParsePath(path);
        var buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);

        try
        {
            var state = new JsonParserState();
            var resultBuffer = new MemoryStream();
            var bytesInBuffer = 0;

            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer.AsMemory(bytesInBuffer, _bufferSize - bytesInBuffer), cancellationToken);
                if (bytesRead == 0 && bytesInBuffer == 0)
                    break;

                bytesInBuffer += bytesRead;
                var consumed = ProcessBuffer(buffer.AsSpan(0, bytesInBuffer), ref state, segments, resultBuffer, optimizeProperties);

                if (state.HasCompleteResult)
                {
                    resultBuffer.Position = 0;
                    var result = await JsonSerializer.DeserializeAsync<T>(resultBuffer, cancellationToken: cancellationToken);
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
        var segments = ParsePath(path);
        var state = new JsonParserState();
        var resultBuffer = new MemoryStream();

        ProcessBuffer(json, ref state, segments, resultBuffer, optimizeProperties);

        if (resultBuffer.Length == 0)
            return default!;

        resultBuffer.Position = 0;
        return JsonSerializer.Deserialize<T>(resultBuffer.ToArray())!;
    }

    public List<T> ParseAll<T>(ReadOnlySpan<byte> json, string path, bool optimizeProperties = true)
    {
        var segments = ParsePath(path);
        var results = new List<T>();
        var state = new JsonParserState();
        var resultBuffer = new MemoryStream();
        var offset = 0;

        while (offset < json.Length)
        {
            var consumed = ProcessBuffer(json[offset..], ref state, segments, resultBuffer, optimizeProperties);
            offset += consumed;

            if (state.HasCompleteResult)
            {
                resultBuffer.Position = 0;
                var result = JsonSerializer.Deserialize<T>(resultBuffer.ToArray());
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
        var reader = new Utf8JsonReader(buffer, isFinalBlock: false, state.JsonReaderState);
        var consumed = 0;

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
                        var propertyName = reader.GetString();
                        if (currentSegment?.PropertyName != null &&
                            currentSegment.Value.PropertyName!.Equals(propertyName, StringComparison.Ordinal))
                        {
                            state.DepthAtMatch[state.MatchDepth] = state.Depth;
                            state.MatchDepth++;
                        }
                        else if (optimizeProperties && state.MatchDepth < segments.Count)
                        {
                            SkipValue(ref reader);
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
        using var writer = new Utf8JsonWriter(output);
        WriteCurrentValue(ref reader, writer);
        writer.Flush();
    }

    private static void WriteCurrentValue(ref Utf8JsonReader reader, Utf8JsonWriter writer)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                writer.WriteStartObject();
                var objectDepth = reader.CurrentDepth;
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
                var arrayDepth = reader.CurrentDepth;
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
                if (reader.TryGetInt64(out var longValue))
                    writer.WriteNumberValue(longValue);
                else if (reader.TryGetDouble(out var doubleValue))
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
            var depth = reader.CurrentDepth;
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
        var segments = new List<PathSegment>();
        var span = path.AsSpan();
        var i = 0;

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

                var start = i;
                while (i < span.Length && span[i] != '.' && span[i] != '[')
                    i++;

                if (i > start)
                {
                    var propName = span[start..i].ToString();
                    segments.Add(new PathSegment { PropertyName = propName });
                }
            }
            else if (span[i] == '[')
            {
                i++;
                var start = i;
                while (i < span.Length && span[i] != ']')
                    i++;

                var indexStr = span[start..i].ToString().Trim();
                i++;

                if (indexStr == "*")
                    segments.Add(new PathSegment { IsWildcard = true });
                else if (int.TryParse(indexStr, out var index))
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
