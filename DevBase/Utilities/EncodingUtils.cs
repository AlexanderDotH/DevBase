using System.Text;

namespace DevBase.Utilities
{
    public static class EncodingUtils
    {
        public static Encoding GetEncoding(Memory<byte> buffer) => GetEncoding(buffer.ToArray());
        public static Encoding GetEncoding(ReadOnlySpan<byte> buffer) => GetEncoding(buffer.ToArray());
        
        public static Encoding GetEncoding(byte[] buffer)
        {
            using MemoryStream memoryStream = new MemoryStream(buffer);
            using StreamReader streamReader = new StreamReader(memoryStream, true);
            
            return streamReader.CurrentEncoding;
        }
    }
}
