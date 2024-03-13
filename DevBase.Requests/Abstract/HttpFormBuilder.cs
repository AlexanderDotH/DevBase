namespace DevBase.Requests.Abstract;

#pragma warning disable S2436
public abstract class HttpFormBuilder<T, TKeyK, TKeyV> 
    : HttpBodyBuilder<T> where T 
    : HttpFormBuilder<T, TKeyK, TKeyV>
{
    protected List<KeyValuePair<TKeyK, TKeyV>> FormData { get; private set; }
    
    protected HttpFormBuilder()
    {
        FormData = new List<KeyValuePair<TKeyK, TKeyV>>();
    }

    protected void AddFormElement(TKeyK key, TKeyV value) => 
        FormData.Add(KeyValuePair.Create(key, value));

    protected void RemoveFormElement(int index) => FormData.RemoveAt(index);
    protected void RemoveFormElementKey(TKeyK key) => this.FormData.RemoveAll((kv) => kv.Key.Equals(key));
    protected void RemoveFormElementValue(TKeyK value) => this.FormData.RemoveAll((kv) => kv.Value.Equals(value));
}
#pragma warning restore S2436