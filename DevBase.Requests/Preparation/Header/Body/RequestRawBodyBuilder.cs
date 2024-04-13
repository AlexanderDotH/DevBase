using System.Net;
using System.Text;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Preparation.Header.Body.Content;

namespace DevBase.Requests.Preparation.Header.Body;

public class RequestRawBodyBuilder : HttpBodyBuilder<RequestRawBodyBuilder>
{
    private bool ValidateInput { get; set; }
    
    public RequestRawBodyBuilder(bool validateData)
    {
        this.ValidateInput = validateData;
    }

    public RequestRawBodyBuilder WithText(string textContent, Encoding encoding) => 
        With<StringRequestContent, string>(textContent, encoding);
    
    public RequestRawBodyBuilder WithJson(string jsonContent, Encoding encoding) => 
        With<JsonRequestContent, string>(jsonContent, encoding);
    
    public RequestRawBodyBuilder WithBuffer(byte[] buffer) => 
        With<BufferRequestContent, byte[]>(buffer);

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