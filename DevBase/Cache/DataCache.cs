﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;
using DevBase.Web.RequestData;

namespace DevBase.Cache
{
    public class DataCache<K,V>
    {
        private readonly int _expirationMS;

        private readonly ATupleList<K, CacheElement<V>> _cache;

        public DataCache(int expirationMS)
        {
            this._cache = new ATupleList<K, CacheElement<V>>();
            this._expirationMS = expirationMS;
        }

        public DataCache() : this(2000) {}

        public void WriteToCache(K key, V value)
        {
            this._cache.Add(key, new CacheElement<V>(value, DateTimeOffset.Now.AddMilliseconds(this._expirationMS).ToUnixTimeMilliseconds()));
        }

        public V DataFromCache(K key)
        {
            RefreshExpirationDate();

            CacheElement<V> element = this._cache.FindEntrySafe(key);

            if (element != null)
                return element.Value;

            return default;
        }

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
