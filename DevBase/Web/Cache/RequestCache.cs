using DevBase.Cache;

namespace DevBase.Web.Cache
{
    public class RequestCache : DataCache<Uri, ResponseData.ResponseData>
    {
        private bool _isCachingAllowed;

        private static RequestCache _instance;

        public RequestCache()
        {
            this._isCachingAllowed = false;
        }

        public bool CachingAllowed
        {
            get { return this._isCachingAllowed; }
            set { this._isCachingAllowed = value; }
        }

        public static RequestCache INSTANCE
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RequestCache();
                }

                return _instance;
            }
        }
    }
}
