using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Objects;
using DevBase.Requests.Struct;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.Body;


public class RequestEncodedFormHeaderBuilder : HttpFormBuilder<RequestEncodedFormHeaderBuilder, string, object>
{
    public Memory<byte> Bounds { get; private set; }
    public Memory<byte> Separator { get; private set; }
    public Memory<byte> Tail { get; private set; }
    
    public RequestEncodedFormHeaderBuilder()
    {
        ContentDispositionBounds bounds = ContentDispositionUtils.GetBounds();

        Bounds = bounds.Bounds;
        Separator = bounds.Separator;
        Tail = bounds.Tail;
    }
    
    protected override Action BuildAction => () =>
    {
        List<Memory<byte>> buffer = new List<Memory<byte>>();
        
        for (var i = 0; i < FormData.Count; i++)
        {
            KeyValuePair<string, object> formEntry = FormData[i];

            if (!(formEntry.Value is string ))
                continue;
            
            buffer.Add(ContentDispositionUtils.NewLine);
            buffer.Add(Separator);
            buffer.Add(ContentDispositionUtils.NewLine);
   
            if (formEntry.Value is string textEntry)
                buffer.Add(ContentDispositionUtils.FromValue(formEntry.Key, textEntry));
        }

        buffer.Add(ContentDispositionUtils.NewLine);
        buffer.Add(Tail);

        Buffer = BufferUtils.Combine(buffer);
    };
    
    public RequestEncodedFormHeaderBuilder RemoveEntryAt(int index)
    {
        RemoveFormElement(index);
        return this;
    }

    public RequestEncodedFormHeaderBuilder Remove(string fieldName)
    {
        RemoveFormElement(fieldName);
        return this;
    }
    
    public object this[string fieldName]
    {
        set
        {
            if (value is null)
            {
                RemoveFormElement(fieldName);
                return;
            }
            
            if (!(value is string))
                throw new ElementValidationException(EnumValidationReason.DataMismatch);

            if (value is string textContent)
                AddText(fieldName, textContent);
        }
    }
    
    public RequestEncodedFormHeaderBuilder AddText(string key, string value)
    {
        AddFormElement(key, value);
        return this;
    }
}