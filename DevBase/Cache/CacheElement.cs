using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Cache
{
    [Serializable]
    public class CacheElement<V>
    {
        private V _value;
        private long _expirationDate;

        public CacheElement(V value, long expirationDate)
        {
            this._value = value;
            this._expirationDate = expirationDate;
        }

        public V Value
        {
            get => this._value;
            set => this._value = value;
        }

        public long ExpirationDate
        {
            get => this._expirationDate;
            set => this._expirationDate = value;
        }
    }
}
