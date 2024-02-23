using System.Buffers;
using System.Diagnostics;
using System.Text;
using DevBase.Requests.Objects;
using DevBase.Requests.Struct;

namespace DevBase.Requests.Utils;

public class ContentDispositionUtils
{
    private static Memory<char> _boundaryLine = "--------------------------".ToCharArray();
    private static Memory<char> _boundaryTail = "--".ToCharArray();

    private static Memory<char> _prefix = "Content-Disposition".ToCharArray();
    private static Memory<char> _contentType = "Content-Type".ToCharArray();
    private static Memory<char> _formData = "form-data".ToCharArray();
    private static Memory<char> _name = "name".ToCharArray();
    private static Memory<char> _fileName = "filename".ToCharArray();

    // \r\n or \n or \r based on the os
    private static Memory<char> _newLine = Environment.NewLine.ToCharArray();

    public static readonly Memory<byte> NewLine = Encoding.UTF8.GetBytes(Environment.NewLine);

    public static ContentDispositionBounds GetBounds()
    {
        long ticks = DateTimeOffset.Now.Ticks;

        Memory<byte> tail = GetTail(ticks);
        Memory<byte> boundary = GetBoundary(tail);
        Memory<byte> separator = GetSeparator(tail);
        
        return new ContentDispositionBounds()
        {
            Bounds = boundary,
            Separator = separator,
            Tail = tail
        };
    }

    public static Memory<byte> GetTail(long ticks)
    {
        StringBuilder tailBuilder = new StringBuilder(45);

        tailBuilder.Append(_boundaryTail);
        tailBuilder.Append(_boundaryLine);
        tailBuilder.Append(ticks);
        tailBuilder.Append(_boundaryTail);

        return Encoding.UTF8.GetBytes(tailBuilder.ToString());
    }
    
    public static Memory<byte> GetBoundary(Memory<byte> tail)
    {
        return tail.Slice(2, tail.Length - 2);
    }
    
    public static Memory<byte> GetSeparator(Memory<byte> tail)
    {
        return tail.Slice(0, tail.Length - 2);
    }
    
    public static Memory<byte> FromValue(ReadOnlySpan<char> fieldName, ReadOnlySpan<char> fieldValue)
    {
        StringBuilder stringBuilder = new StringBuilder(70);
        
        // "Content-Disposition: "
        stringBuilder.Append(_prefix);
        stringBuilder.Append(':');
        stringBuilder.Append(' ');
        
        // "form-data; "
        stringBuilder.Append(_formData);
        stringBuilder.Append(';');
        stringBuilder.Append(' ');

        // "name="fieldName""
        stringBuilder.Append(_name);
        stringBuilder.Append('=');
        stringBuilder.Append('\"');
        stringBuilder.Append(fieldName);
        stringBuilder.Append('\"');

        // 2 new-lines
        stringBuilder.Append(_newLine);
        stringBuilder.Append(_newLine);

        // fieldValue
        stringBuilder.Append(fieldValue);

        return Encoding.UTF8.GetBytes(stringBuilder.ToString());
    }
    
    public static Memory<byte> FromFile(ReadOnlySpan<char> fieldName, MimeFileObject mimeFileObject)
    {
        StringBuilder fileBuilder = new StringBuilder(70);

        // "Content-Disposition: "
        fileBuilder.Append(_prefix);
        fileBuilder.Append(':');
        fileBuilder.Append(' ');

        // "form-data; "
        fileBuilder.Append(_formData);
        fileBuilder.Append(';');
        fileBuilder.Append(' ');

        // name="fieldName"; "
        fileBuilder.Append(_name);
        fileBuilder.Append('=');
        fileBuilder.Append('\"');
        fileBuilder.Append(fieldName);
        fileBuilder.Append('\"');
        fileBuilder.Append(';');
        fileBuilder.Append(' ');
        
        // "filename="fileName"; "
        fileBuilder.Append(_fileName);
        fileBuilder.Append('=');
        fileBuilder.Append('\"');
        fileBuilder.Append(mimeFileObject.FileInfo.Name);
        fileBuilder.Append('\"');
        fileBuilder.Append(' ');
        
        // new line
        fileBuilder.Append(_newLine);
        
        // "Content-Type: mimeType"
        fileBuilder.Append(_contentType);
        fileBuilder.Append(':');
        fileBuilder.Append(' ');
        fileBuilder.Append(mimeFileObject.MimeType);

        // 2 new-lines
        fileBuilder.Append(_newLine);
        fileBuilder.Append(_newLine);

        byte[] textPart = Encoding.UTF8.GetBytes(fileBuilder.ToString());
        
        byte[] buffer = new byte[textPart.Length + mimeFileObject.Buffer.Length];
        
        Array.Copy(textPart, buffer, textPart.Length);
        Array.Copy(mimeFileObject.Buffer.ToArray(), 0, buffer, textPart.Length, mimeFileObject.Buffer.Length);
        
        return buffer;
    }
}