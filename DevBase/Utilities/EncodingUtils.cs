using System.Text;

namespace DevBase.Utilities
{
    /// <summary>
    /// Provides utility methods for encoding detection.
    /// </summary>
    public static class EncodingUtils
    {
        /// <summary>
        /// Detects the encoding of a byte buffer.
        /// </summary>
        /// <param name="buffer">The memory buffer.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(Memory<byte> buffer) => GetEncoding(buffer.ToArray());
        
        /// <summary>
        /// Detects the encoding of a byte buffer.
        /// </summary>
        /// <param name="buffer">The read-only span buffer.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(ReadOnlySpan<byte> buffer) => GetEncoding(buffer.ToArray());
        
        /// <summary>
        /// Detects the encoding of a byte array using a StreamReader.
        /// </summary>
        /// <param name="buffer">The byte array.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(byte[] buffer)
        {
            using MemoryStream memoryStream = new MemoryStream(buffer);
            using StreamReader streamReader = new StreamReader(memoryStream, true);
            
            return streamReader.CurrentEncoding;
        }
    }
}
