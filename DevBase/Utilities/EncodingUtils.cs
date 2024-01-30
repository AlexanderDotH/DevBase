using System.Text;

namespace DevBase.Utilities
{
    public static class EncodingUtils
    {
        public static Encoding GetEncoding(Memory<byte> buffer)
        {
            byte[] bufferArray = buffer.ToArray();

            using MemoryStream memoryStream = new MemoryStream(bufferArray);
            using StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8, true);
            
            return streamReader.CurrentEncoding;
        }
    }
}
