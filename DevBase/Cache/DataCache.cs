using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBase.Cache
{
    public class DataCache<K,V>
    {
        private int _expirationMS;

        private GenericTupleList<K, CacheElement<V>> _cache;

        public DataCache(int expirationMS)
        {
            this._cache = new GenericTupleList<K, CacheElement<V>>();
            this._expirationMS = expirationMS;
        }

        public DataCache() : this(2000) {}

        public void WriteToCache(K key, V value)
        {
            this._cache.Add(key, new CacheElement<V>(value, DateTimeOffset.Now.AddMilliseconds(this._expirationMS).ToUnixTimeMilliseconds()));
        }

        public V DataFromCache(K key)
        {
            RefreshExpirartionDate();

            CacheElement<V> element = this._cache.FindEntry(key);

            if (element != null)
                return element.Value;

            return (V)new object();
        }

        public GenericList<V> DataFromCacheAsList(K key)
        {
            RefreshExpirartionDate();

            GenericList<CacheElement<V>> entries = this._cache.FindEntries(key);
            GenericList<V> returnElements = new GenericList<V>();

            for (int i = 0; i < entries.Length; i++)
            {
                returnElements.Add(entries.Get(i).Value);
            }

            return returnElements;
        }

        private void RefreshExpirartionDate()
        {
            GenericTupleList<K, CacheElement<V>> _copyOfCache = new GenericTupleList<K, CacheElement<V>>(this._cache);

            for (int i = 0; i < _copyOfCache.Length; i++)
            {
                Tuple<K, CacheElement<V>> currentElement = _copyOfCache.Get(i);

                if (currentElement.Item2.ExpirationDate > DateTimeOffset.Now.ToUnixTimeMilliseconds())
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
