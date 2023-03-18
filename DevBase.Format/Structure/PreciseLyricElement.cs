namespace DevBase.Format.Structure;

public class PreciseLyricElement
{
    private long _startTimestamp;
    private long _endTimeStamp;
    private string _text;

    public PreciseLyricElement(long startTimestamp, long endTimeStamp, string text)
    {
        this._startTimestamp = startTimestamp;
        this._endTimeStamp = endTimeStamp;
        this._text = text;
    }

    public long StartTimestamp
    {
        get => _startTimestamp;
        set => _startTimestamp = value;
    }

    public long EndTimeStamp
    {
        get => _endTimeStamp;
        set => _endTimeStamp = value;
    }

    public string Text
    {
        get => _text;
        set => _text = value;
    }
}