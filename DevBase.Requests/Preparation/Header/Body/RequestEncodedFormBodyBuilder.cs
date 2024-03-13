using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Objects;
using DevBase.Requests.Struct;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.Body;


public class RequestEncodedFormBodyBuilder : HttpFormBuilder<RequestEncodedFormBodyBuilder, string, string>
{
    public RequestEncodedFormBodyBuilder() { }
    
    protected override Action BuildAction => () =>
    {
        Buffer = ContentDispositionUtils.Combine(this.FormData);
    };
    
    public RequestEncodedFormBodyBuilder RemoveEntryAt(int index)
    {
        RemoveFormElement(index);
        return this;
    }

    public RequestEncodedFormBodyBuilder Remove(string fieldName)
    {
        RemoveFormElementKey(fieldName);
        return this;
    }
    
    public string this[string fieldName]
    {
        set
        {
            if (value is null)
            {
                RemoveFormElementKey(fieldName);
                return;
            }
            
            if (value == null || value.Length == 0)
                return;
            
            AddText(fieldName, value);
        }
    }
    
    public RequestEncodedFormBodyBuilder AddText(string key, string value)
    {
        AddFormElement(key, value);
        return this;
    }
}