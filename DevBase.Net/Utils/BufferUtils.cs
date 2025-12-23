using System.Data;
using System.Text;

namespace DevBase.Net.Utils;

public class BufferUtils
{
    public static bool HasPreamble(ReadOnlySpan<byte> buffer, Encoding encoding)
    {
        ReadOnlySpan<byte> preamble = encoding.GetPreamble();

        if (buffer.Length < preamble.Length)
            throw new EvaluateException("The buffer is < than the preamble");
        
        for (int i = 0; i < preamble.Length; i++)
        {
            if (!preamble[i].Equals(buffer[i]))
                return false;
        }

        return true;
    }
    
    public static Memory<byte> Combine(IEnumerable<Memory<byte>> buffer)
    {
        List<byte> combined = new List<byte>();

        foreach (Memory<byte> memory in buffer)
            combined.AddRange(memory.ToArray());

        return combined.ToArray();
    }
}