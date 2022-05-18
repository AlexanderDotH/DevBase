using System;
using System.Net;
using System.Text;

namespace DevBase.Web.RequestData
{
    public class RequestData
    {
        private RequestMethod _requestMethod;
        private ContentType[] _contentType;
        private EncodingType[] _encodingType;
        private byte[] _content;
        private Uri _uri;
        private string _userAgent;
        private string _accept;
        private WebHeaderCollection _header;

        public RequestData(Uri uri, RequestMethod requestMethod, ContentType[] contentType, EncodingType[] encodingType, byte[] content, string userAgent)
        {
            this._uri = uri;
            this._content = content;
            this._requestMethod = requestMethod;
            this._contentType = contentType;
            this._encodingType = encodingType;
            this._userAgent = userAgent;
            this._header = new WebHeaderCollection();
        }

        public RequestData(Uri uri, RequestMethod requestMethod, ContentType[] contentType, EncodingType[] encodingType, string content, string userAgent) :
            this(
                uri,
                requestMethod,
                contentType,
                encodingType,
                Encoding.Default.GetBytes(content),
                userAgent
                ) { }

        public RequestData(Uri uri, RequestMethod requestMethod, ContentType[] contentType, EncodingType[] encodingType, string content) : 
            this(
                uri, 
                requestMethod, 
                contentType, 
                encodingType,
                Encoding.Default.GetBytes(content), 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
                ) { }

        public RequestData(string uri, string content) : 
            this(
                new Uri(uri), 
                RequestMethod.GET, 
                new ContentType[] { Web.RequestData.ContentType.HTML }, 
                new EncodingType[] { Web.RequestData.EncodingType.UTF8 },
                Encoding.Default.GetBytes(content), 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
                ) { }

        public RequestData(string uri) :
           this(
               new Uri(uri), 
               RequestMethod.GET,
               new ContentType[] { Web.RequestData.ContentType.HTML },
               new EncodingType[] { Web.RequestData.EncodingType.UTF8 },
               Encoding.Default.GetBytes(string.Empty), 
               "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
               ) { }

        public string ConvertFromContentType(string existingContentType, ContentType[] requestType)
        {
            for (int i = 0; i < requestType.Length; i++)
            {
                existingContentType += ContentTypeToString(requestType[i]) + (i == requestType.Length ? string.Empty : "; ");
            }

            return existingContentType;
        }

        public string ConvertFromEncodingTypes(string existingContentType, EncodingType[] encodingType)
        {
            existingContentType += "charset=";
            for (int i = 0; i < encodingType.Length; i++)
            {
                existingContentType += ContentEncodingType(encodingType[i]) + (i == encodingType.Length ? ";" : ", ");
            }

            return existingContentType;
        }

        private string ContentTypeToString(ContentType contentType)
        {
            switch (contentType)
            {
                case Web.RequestData.ContentType.JSON:
                    return "application/json";
                case Web.RequestData.ContentType.FORM:
                    return "application/x-www-form-urlencoded";
            }

            return string.Empty;
        }

        private string ContentEncodingType(EncodingType contentType)
        {
            switch (contentType)
            {
                case EncodingType.UTF8:
                    return "utf-8";
            }

            return string.Empty;
        }

        public RequestMethod RequestMethod
        {
            get { return this._requestMethod; }
            set { this._requestMethod = value; }
        }

        public byte[] Content
        {
            get { return _content; }
            set { this._content = value; }
        }

        public Uri Uri
        {
            get { return this._uri; }
            set { this._uri = value; }
        }

        public string UserAgent
        {
            get { return this._userAgent; }
            set { this._userAgent = value; }
        }

        public string Accept
        {
            get { return this._accept; }
            set { this._accept = value; }
        }

        public WebHeaderCollection Header
        {
            get { return this._header; }
            set { this._header = value; }
        }

        public ContentType[] ContentType
        {
            get { return this._contentType; }
            set { this._contentType = value; }
        }

        public EncodingType[] EncodingTypes
        {
            get { return this._encodingType; }
            set { this._encodingType = value; }
        }
    }
}
