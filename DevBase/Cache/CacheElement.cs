using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Cache
{
    /// <summary>
    /// Represents an element in the cache with a value and an expiration timestamp.
    /// </summary>
    /// <typeparam name="TV">The type of the value.</typeparam>
    [Serializable]
    public class CacheElement<TV>
    {
        private TV _value;
        private long _expirationDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheElement{TV}"/> class.
        /// </summary>
        /// <param name="value">The value to cache.</param>
        /// <param name="expirationDate">The expiration timestamp in milliseconds.</param>
        public CacheElement(TV value, long expirationDate)
        {
            this._value = value;
            this._expirationDate = expirationDate;
        }

        /// <summary>
        /// Gets or sets the cached value.
        /// </summary>
        public TV Value
        {
            get => this._value;
            set => this._value = value;
        }

        /// <summary>
        /// Gets or sets the expiration date in Unix milliseconds.
        /// </summary>
        public long ExpirationDate
        {
            get => this._expirationDate;
            set => this._expirationDate = value;
        }
    }
}
