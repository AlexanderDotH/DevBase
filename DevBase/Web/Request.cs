using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Web.RequestData;
using System.Net;
using System.IO;
using System.Net.Cache;
using DevBase.Enums;
using DevBase.Utilities;
using DevBase.Web.Cache;

namespace DevBase.Web
{
    public class Request
    {
        private readonly RequestData.RequestData _requestData;

        public Request(RequestData.RequestData requestData)
        {
            this._requestData = requestData;
        }

        public Request(string url) : this(new RequestData.RequestData(url)) {}

        public ResponseData.ResponseData GetResponse(bool allowCaching = true)
        {
            Task<ResponseData.ResponseData> response = GetResponseAsync(allowCaching);
            return response.GetAwaiter().GetResult();
        }

        public async Task<ResponseData.ResponseData> GetResponseAsync(bool allowCaching = true)
        {
            if (allowCaching)
            {
                if (RequestCache.INSTANCE != null && RequestCache.INSTANCE.CachingAllowed)
                {
                    if (RequestCache.INSTANCE.IsInCache(this._requestData.Uri))
                    {
                        return RequestCache.INSTANCE.DataFromCache(this._requestData.Uri);
                    }
                }
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this._requestData.Uri);

            request.Headers = this._requestData.Header;
            request.Method = this._requestData.RequestMethod.ToString();
            request.ContentType = this._requestData.ContentTypeHolder.ContentType;
            request.UserAgent = this._requestData.UserAgent;
            request.Accept = this._requestData.AcceptTypeHolder.GetAccept();
            request.CookieContainer = this._requestData.CookieContainer;

            if (this._requestData.RequestMethod == EnumRequestMethod.POST && 
                this._requestData.Content != null &&
                this._requestData.Content.Length != 0)
            {
                request.ContentLength = this._requestData.Content.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    await requestStream.WriteAsync(this._requestData.Content, 0, this._requestData.Content.Length);
                }
            }

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            ResponseData.ResponseData responseData = new ResponseData.ResponseData(response);

            if (allowCaching)
            {
                if (RequestCache.INSTANCE != null && RequestCache.INSTANCE.CachingAllowed)
                {
                    if (!RequestCache.INSTANCE.IsInCache(this._requestData.Uri))
                    {
                        RequestCache.INSTANCE.WriteToCache(this._requestData.Uri, responseData);
                    }
                }
            }

            return responseData;
        }
    }
}
