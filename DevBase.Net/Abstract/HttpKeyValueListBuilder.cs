namespace DevBase.Net.Abstract;

/// <summary>
/// Abstract base class for building lists of key-value pairs for HTTP bodies (e.g. form data).
/// </summary>
/// <typeparam name="T">The specific builder type.</typeparam>
/// <typeparam name="TKeyK">The type of the keys.</typeparam>
/// <typeparam name="TKeyV">The type of the values.</typeparam>
#pragma warning disable S2436
public abstract class HttpKeyValueListBuilder<T, TKeyK, TKeyV> 
    : HttpBodyBuilder<T> where T 
    : HttpKeyValueListBuilder<T, TKeyK, TKeyV>
{
    /// <summary>
    /// Gets the list of entries.
    /// </summary>
    protected List<KeyValuePair<TKeyK, TKeyV>> Entries { get; private set; }
    
    /// <summary>
    /// Gets a read-only view of the entries.
    /// </summary>
    public IReadOnlyList<KeyValuePair<TKeyK, TKeyV>> GetEntries() => Entries.AsReadOnly();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpKeyValueListBuilder{T, TKeyK, TKeyV}"/> class.
    /// </summary>
    protected HttpKeyValueListBuilder()
    {
        Entries = new List<KeyValuePair<TKeyK, TKeyV>>();
    }

    /// <summary>
    /// Adds a new key-value pair entry.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    protected void AddEntry(TKeyK key, TKeyV value) => 
        Entries.Add(KeyValuePair.Create(key, value));

    /// <summary>
    /// Adds a new entry if the key doesn't exist, otherwise updates the existing entry.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    protected void AddOrSetEntry(TKeyK key, TKeyV value)
    {
        if (!this.AnyEntry(key))
        {
            this.AddEntry(key, value);
            return;
        }
        
        this.SetEntryValue(key, value);
    }
    
    /// <summary>
    /// Removes an entry at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the entry to remove.</param>
    protected void RemoveEntry(int index) => Entries.RemoveAt(index);
    
    /// <summary>
    /// Removes all entries with the specified key.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    protected void RemoveEntryKey(TKeyK key) => 
        this.Entries.RemoveAll((kv) => KeyEquals(kv.Key, key));
    
    /// <summary>
    /// Removes all entries with the specified value.
    /// </summary>
    /// <param name="value">The value to remove.</param>
    protected void RemoveEntryValue(TKeyK value) => 
        this.Entries.RemoveAll((kv) => kv.Value!.Equals(value));
    
    /// <summary>
    /// Gets the value associated with the specified key. Returns default if not found.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The value.</returns>
    protected TKeyV GetEntryValue(TKeyK key)
    {
        for (int i = 0; i < this.Entries.Count; i++)
        {
            if (KeyEquals(this.Entries[i].Key, key))
                return this.Entries[i].Value;
        }
        return default!;
    }

    /// <summary>
    /// Gets the value at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The value.</returns>
    protected TKeyV GetEntryValue(int index) =>
        this.Entries[index].Value;
    
    /// <summary>
    /// Sets the value for the specified key. Only updates the first occurrence.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The new value.</param>
    protected void SetEntryValue(TKeyK key, TKeyV value)
    {
        for (int i = 0; i < this.Entries.Count; i++)
        {
            if (KeyEquals(this.Entries[i].Key, key))
            {
                this.Entries[i] = KeyValuePair.Create(this.Entries[i].Key, value);
                return;
            }
        }
    }
    
    /// <summary>
    /// Sets the value at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The new value.</param>
    protected void SetEntryValue(int index, TKeyV value)
    {
        TKeyK entryValue = this.Entries[index].Key;
        this.Entries[index] = KeyValuePair.Create(entryValue, value);
    }
    
    /// <summary>
    /// Checks if any entry exists with the specified key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if exists, false otherwise.</returns>
    protected bool AnyEntry(TKeyK key)
    {
        for (int i = 0; i < this.Entries.Count; i++)
        {
            if (KeyEquals(this.Entries[i].Key, key))
                return true;
        }
        return false;
    }
    
    private static bool KeyEquals(TKeyK? a, TKeyK? b)
    {
        if (a is string strA && b is string strB)
            return string.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);
        
        return EqualityComparer<TKeyK>.Default.Equals(a, b);
    }
}
#pragma warning restore S2436