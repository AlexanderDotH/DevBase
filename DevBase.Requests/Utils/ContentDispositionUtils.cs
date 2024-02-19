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

    public static ReadOnlySpan<char> FromValue(ReadOnlySpan<char> fieldName, ReadOnlySpan<char> fieldValue)
    {
        StringBuilder stringBuilder = new StringBuilder(70);
        
        // Content-Disposition
        stringBuilder.Append(_prefix);
        
        // ": "
        stringBuilder.Append(':');
        stringBuilder.Append(' ');
        
        // form-data
        stringBuilder.Append(_formData);
        
        // "; "
        stringBuilder.Append(';');
        stringBuilder.Append(' ');

        // name="
        stringBuilder.Append(_name);
        stringBuilder.Append('=');
        stringBuilder.Append('\"');
        
        // fieldName
        stringBuilder.Append(fieldName);
        
        // ""
        stringBuilder.Append('\"');

        // 2 new-lines
        stringBuilder.Append(_newLine);
        stringBuilder.Append(_newLine);

        // fieldValue
        stringBuilder.Append(fieldValue);

        return stringBuilder.ToString();
    }
    
    public static ReadOnlySpan<char> FromFile(ReadOnlySpan<char> fieldName, ReadOnlySpan<char> fileName, MimeFileObject mimeFileObject)
    {
        StringBuilder fileBuilder = new StringBuilder(70);

        // Content-Disposition
        fileBuilder.Append(_prefix);
        
        // ": "
        fileBuilder.Append(':');
        fileBuilder.Append(' ');

        // form-data
        fileBuilder.Append(_formData);
        
        // "; "
        fileBuilder.Append(';');
        fileBuilder.Append(' ');

        // name="
        fileBuilder.Append(_name);
        fileBuilder.Append('=');
        fileBuilder.Append('\"');
        
        // fieldName
        fileBuilder.Append(fieldName);
        
        // ""; "
        fileBuilder.Append('\"');
        fileBuilder.Append(';');
        fileBuilder.Append(' ');
        
        // filename="
        fileBuilder.Append(_fileName);
        fileBuilder.Append('=');
        fileBuilder.Append('\"');
        
        // "fileName""
        fileBuilder.Append(fileName);
        fileBuilder.Append('\"');
        
        // 2 new-linex
        fileBuilder.Append(_newLine);
        fileBuilder.Append(_newLine);
        
        // "Content-Type: "
        fileBuilder.Append(_contentType);
        fileBuilder.Append(':');
        fileBuilder.Append(' ');
        
        // 

        return fileBuilder.ToString();
    }
}