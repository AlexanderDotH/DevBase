namespace DevBase.Net.Struct;

public ref struct ContentDispositionBounds
{
    public Memory<byte> Bounds { get; set; }
    public Memory<byte> Separator { get; set; }

    public Memory<byte> Tail { get; set; }
}