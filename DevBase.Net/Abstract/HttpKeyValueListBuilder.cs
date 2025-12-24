namespace DevBase.Net.Abstract;

#pragma warning disable S2436
public abstract class HttpKeyValueListBuilder<T, TKeyK, TKeyV> 
    : HttpBodyBuilder<T> where T 
    : HttpKeyValueListBuilder<T, TKeyK, TKeyV>
{
    protected List<KeyValuePair<TKeyK, TKeyV>> Entries { get; private set; }
    
    public IReadOnlyList<KeyValuePair<TKeyK, TKeyV>> GetEntries() => Entries.AsReadOnly();
    
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
        this.Entries.RemoveAll((kv) => KeyEquals(kv.Key, key));
    
    protected void RemoveEntryValue(TKeyK value) => 
        this.Entries.RemoveAll((kv) => kv.Value!.Equals(value));
    
    protected TKeyV GetEntryValue(TKeyK key) => 
        this.Entries.FirstOrDefault(e => KeyEquals(e.Key, key)).Value;

    protected TKeyV GetEntryValue(int index) =>
        this.Entries[index].Value;
    
    protected void SetEntryValue(TKeyK key, TKeyV value)
    {
        int index = this.Entries
            .FindIndex(e => KeyEquals(e.Key, key));
        
        if (index >= 0)
        {
            TKeyK existingKey = this.Entries[index].Key;
            this.Entries[index] = KeyValuePair.Create(existingKey, value);
        }
    }
    
    protected void SetEntryValue(int index, TKeyV value)
    {
        TKeyK entryValue = this.Entries[index].Key;
        this.Entries[index] = KeyValuePair.Create(entryValue, value);
    }
    
    protected bool AnyEntry(TKeyK key) => 
        this.Entries.Exists(e => KeyEquals(e.Key, key));
    
    private static bool KeyEquals(TKeyK? a, TKeyK? b)
    {
        if (a is string strA && b is string strB)
            return string.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);
        
        return EqualityComparer<TKeyK>.Default.Equals(a, b);
    }
}
#pragma warning restore S2436