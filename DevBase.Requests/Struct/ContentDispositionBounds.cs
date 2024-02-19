namespace DevBase.Requests.Struct;

public ref struct ContentDispositionBounds
{
    public Memory<byte> Bounds { get; set; }
    public Memory<byte> Separator { get; set; }

    public Memory<byte> Tail { get; set; }
}