using System.Text.Json;

namespace DevBase.Requests.Parsing;

/// <summary>
/// Maintains state during streaming JSON parsing.
/// </summary>
internal struct JsonParserState
{
    public JsonReaderState JsonReaderState;
    public int Depth;
    public int MatchDepth;
    public int[] DepthAtMatch;
    public int ArrayIndex;
    public bool HasCompleteResult;

    public JsonParserState()
    {
        JsonReaderState = default;
        Depth = 0;
        MatchDepth = 0;
        DepthAtMatch = new int[32];
        ArrayIndex = 0;
        HasCompleteResult = false;
    }
}
