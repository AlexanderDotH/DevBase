using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Cache
{
    /// <summary>
    /// A generic data cache implementation with expiration support.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    public class DataCache<K,V>
    {
        private readonly int _expirationMS;

        private readonly ATupleList<K, CacheElement<V>> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCache{K, V}"/> class.
        /// </summary>
        /// <param name="expirationMS">The cache expiration time in milliseconds.</param>
        public DataCache(int expirationMS)
        {
            this._cache = new ATupleList<K, CacheElement<V>>();
            this._expirationMS = expirationMS;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCache{K, V}"/> class with a default expiration of 2000ms.
        /// </summary>
        public DataCache() : this(2000) {}

        /// <summary>
        /// Writes a value to the cache with the specified key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        public void WriteToCache(K key, V value)
        {
            this._cache.Add(key, new CacheElement<V>(value, DateTimeOffset.Now.AddMilliseconds(this._expirationMS).ToUnixTimeMilliseconds()));
        }

        /// <summary>
        /// Retrieves a value from the cache by key.
        /// Returns default(V) if the key is not found or expired.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The cached value, or default.</returns>
        public V DataFromCache(K key)
        {
            RefreshExpirationDate();

            CacheElement<V> element = this._cache.FindEntrySafe(key);

            if (element != null)
                return element.Value;

            return default;
        }

        /// <summary>
        /// Retrieves all values associated with a key from the cache as a list.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>A list of cached values.</returns>
        public AList<V> DataFromCacheAsList(K key)
        {
            RefreshExpirationDate();

            AList<CacheElement<V>> entries = this._cache.FindEntries(key);
            AList<V> returnElements = new AList<V>();

            for (int i = 0; i < entries.Length; i++)
            {
                returnElements.Add(entries.Get(i).Value);
            }

            return returnElements;
        }

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        public bool IsInCache(K key)
        {
            dynamic v = this._cache.FindEntrySafe(key);

            return v != null;
        }

        private void RefreshExpirationDate()
        {
            ATupleList<K, CacheElement<V>> _copyOfCache = new ATupleList<K, CacheElement<V>>(this._cache);

            for (int i = 0; i < _copyOfCache.Length; i++)
            {
                Tuple<K, CacheElement<V>> currentElement = _copyOfCache.Get(i);

                if (DateTimeOffset.Now.ToUnixTimeMilliseconds() > currentElement.Item2.ExpirationDate)
                {
                    if (this._cache.Contains(currentElement))
                    {
                        this._cache.Remove(currentElement);
                    }
                }
            }
        }

    }
}
