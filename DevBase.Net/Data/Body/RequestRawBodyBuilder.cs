using System.Text;
using DevBase.Net.Abstract;
using DevBase.Net.Data.Body.Content;
using DevBase.Net.Enums;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Data.Body;

public class RequestRawBodyBuilder : HttpBodyBuilder<RequestRawBodyBuilder>
{
    public bool ValidateInput { get; set; } = true;

    public RequestRawBodyBuilder WithText(string textContent, Encoding encoding) => 
        With<StringRequestContent, string>(textContent, encoding);
    
    public RequestRawBodyBuilder WithJson(string jsonContent, Encoding encoding) => 
        With<JsonRequestContent, string>(jsonContent, encoding);
    
    public RequestRawBodyBuilder WithBuffer(byte[] buffer) => 
        With<BufferRequestContent, byte[]>(buffer);

    public RequestRawBodyBuilder UseValidation(bool validateInput)
    {
        ValidateInput = validateInput;
        return this;
    }
    
    private RequestRawBodyBuilder With<TCv, TC>(TC content, Encoding encoding = null) where TCv : RequestContent
    {
        byte[] byteBuffer = Array.Empty<byte>();
        
        if (encoding == null)
            encoding = Encoding.UTF8;
        
        if (content is string text)
            byteBuffer = encoding.GetBytes(text);
        
        if (content is byte[] buffer)
            byteBuffer = buffer;

        if (byteBuffer == null || byteBuffer.Length == 0)
            throw new ElementValidationException(EnumValidationReason.DataMismatch);
        
        if (this.ValidateInput)
        {
            TCv requestContent = default;
            
            if (content is string)
                requestContent = (TCv)Activator.CreateInstance(typeof(TCv), encoding)!;
            
            if (content is byte[])
                requestContent = (TCv)Activator.CreateInstance(typeof(TCv))!;
            
            if (!requestContent!.IsValid(byteBuffer))
                throw new ElementValidationException(EnumValidationReason.InvalidData);
        }

        this.Buffer = byteBuffer;
        
        return this;
    }
    
    protected override Action BuildAction => () =>
    {
        if (this.Buffer.IsEmpty)
            throw new ElementValidationException(EnumValidationReason.Empty);
    };
}