using System.Text;
using DevBase.Requests.Objects;

namespace DevBase.Requests.Builder;

// TODO: Make this class byte[] only. And make entire concatination here
public class ContentDispositionUtils
{
    // "Content-Disposition:"
    private static Memory<char> _prefix = "Content-Disposition".ToCharArray();
    
    // "Content-Type"
    private static Memory<char> _contentType = "Content-Type".ToCharArray();
    
    // "form-data"
    private static Memory<char> _formData = "form-data".ToCharArray();

    // name
    private static Memory<char> _name = "name".ToCharArray();

    // filename
    private static Memory<char> _fileName = "filename".ToCharArray();

    // \r\n or \n or \r based on the os
    private static Memory<char> _newLine = Environment.NewLine.ToCharArray();

    public static ReadOnlySpan<byte> FromValue(ReadOnlySpan<char> fieldName, ReadOnlySpan<char> fieldValue)
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
    
    public static ReadOnlySpan<byte> FromFile(ReadOnlySpan<char> fieldName, MimeFileObject mimeFileObject)
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