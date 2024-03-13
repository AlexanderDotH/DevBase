using DevBase.IO;
using DevBase.Requests.Abstract;
using DevBase.Requests.Enums;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Objects;
using DevBase.Requests.Struct;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Preparation.Header.Body;


public class RequestEncodedFormHeaderBuilder : HttpFormBuilder<RequestEncodedFormHeaderBuilder, string, string>
{
    public RequestEncodedFormHeaderBuilder() { }
    
    protected override Action BuildAction => () =>
    {
        Buffer = ContentDispositionUtils.Combine(this.FormData);
    };
    
    public RequestEncodedFormHeaderBuilder RemoveEntryAt(int index)
    {
        RemoveFormElement(index);
        return this;
    }

    public RequestEncodedFormHeaderBuilder Remove(string fieldName)
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
    
    public RequestEncodedFormHeaderBuilder AddText(string key, string value)
    {
        AddFormElement(key, value);
        return this;
    }
}