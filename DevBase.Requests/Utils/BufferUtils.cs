namespace DevBase.Requests.Utils;

public class BufferUtils
{
    public static Memory<byte> Combine(IEnumerable<Memory<byte>> buffer)
    {
        List<byte> combined = new List<byte>();

        foreach (Memory<byte> memory in buffer)
            combined.AddRange(memory.ToArray());

        return combined.ToArray();
    }
}