using DevBase.Requests.Abstract;
using DevBase.Requests.Utils;

namespace DevBase.Requests.Data.Header.Body;


public class RequestEncodedKeyValueListBodyBuilder : HttpKeyValueListBuilder<RequestEncodedKeyValueListBodyBuilder, string, string>
{
    public RequestEncodedKeyValueListBodyBuilder() { }
    
    protected override Action BuildAction => () =>
    {
        Buffer = ContentDispositionUtils.Combine(this.Entries);
    };
    
    public RequestEncodedKeyValueListBodyBuilder RemoveEntryAt(int index)
    {
        RemoveEntry(index);
        return this;
    }

    public RequestEncodedKeyValueListBodyBuilder Remove(string fieldName)
    {
        RemoveEntryKey(fieldName);
        return this;
    }
    
    #pragma warning disable S2589
    public string this[string fieldName]
    {
        set
        {
            if (value is null)
            {
                RemoveEntryKey(fieldName);
                return;
            }
            
            if (value == null || value.Length == 0)
                return;
            
            AddText(fieldName, value);
        }
    }
    #pragma warning restore S2589
    
    public RequestEncodedKeyValueListBodyBuilder AddText(string key, string value)
    {
        AddEntry(key, value);
        return this;
    }
}