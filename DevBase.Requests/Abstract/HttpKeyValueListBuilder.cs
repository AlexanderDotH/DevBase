namespace DevBase.Requests.Abstract;

#pragma warning disable S2436
public abstract class HttpKeyValueListBuilder<T, TKeyK, TKeyV> 
    : HttpBodyBuilder<T> where T 
    : HttpKeyValueListBuilder<T, TKeyK, TKeyV>
{
    protected List<KeyValuePair<TKeyK, TKeyV>> Entries { get; private set; }
    
    protected HttpKeyValueListBuilder()
    {
        Entries = new List<KeyValuePair<TKeyK, TKeyV>>();
    }

    protected void AddEntry(TKeyK key, TKeyV value) => 
        Entries.Add(KeyValuePair.Create(key, value));

    protected void AddOrSetEntry(TKeyK key, TKeyV value)
    {
        if (!this.AnyEntry(key))
        {
            this.AddEntry(key, value);
            return;
        }
        
        this.SetEntryValue(key, value);
    }
    
    protected void RemoveEntry(int index) => Entries.RemoveAt(index);
    
    protected void RemoveEntryKey(TKeyK key) => 
        this.Entries.RemoveAll((kv) => kv.Key!.Equals(key));
    
    protected void RemoveEntryValue(TKeyK value) => 
        this.Entries.RemoveAll((kv) => kv.Value!.Equals(value));
    
    protected TKeyV GetEntryValue(TKeyK key) => 
        this.Entries.FirstOrDefault(e => e.Key!.Equals(key)).Value;

    protected TKeyV GetEntryValue(int index) =>
        this.Entries[index].Value;
    
    protected void SetEntryValue(TKeyK key, TKeyV value)
    {
        int index = this.Entries
            .FindIndex(e => e.Key!.Equals(key));
        
        this.Entries[index] = KeyValuePair.Create(key, value);
    }
    
    protected void SetEntryValue(int index, TKeyV value)
    {
        TKeyK entryValue = this.Entries[index].Key;
        this.Entries[index] = KeyValuePair.Create(entryValue, value);
    }
    
    protected bool AnyEntry(TKeyK key) => 
        this.Entries.Exists(e => e.Key!.Equals(key));
}
#pragma warning restore S2436