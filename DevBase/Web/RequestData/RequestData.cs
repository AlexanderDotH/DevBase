using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using DevBase.Enums;
using DevBase.Generics;
using DevBase.IO;
using DevBase.Web.RequestData.Data;
using DevBase.Web.RequestData.Types;
using EnumContentType = DevBase.Enums.EnumContentType;

namespace DevBase.Web.RequestData
{
    [Serializable]
    public class RequestData
    {
        private EnumRequestMethod _requestMethod;
        private ContentTypeHolder _contentType;
        private AcceptTypeHolder _acceptTypeHolder;
        private FormDataHolder _formDataHolder;
        private MultipartFormHolder _multipartFormHolder;
        private byte[] _content;
        private Uri _uri;
        private string _userAgent;
        private string _accept;
        private WebHeaderCollection _header;
        private CookieContainer _cookieContainer;

        public RequestData(Uri uri, EnumRequestMethod requestMethod, EnumContentType contentType, string userAgent)
        {
            this._uri = uri;
            this._requestMethod = requestMethod;

            ContentTypeHolder contentTypeHolder = new ContentTypeHolder();
            contentTypeHolder.Set(contentType);
            this._contentType = contentTypeHolder;

            AcceptTypeHolder acceptTypeHolder = new AcceptTypeHolder(contentTypeHolder);
            this._acceptTypeHolder = acceptTypeHolder;

            this._userAgent = userAgent;
            this._cookieContainer = new CookieContainer();
            this._header = new WebHeaderCollection();
        }

        public RequestData(Uri uri, EnumRequestMethod requestMethod, EnumContentType contentType) : 
            this(
                uri, 
                requestMethod, 
                contentType,
                GetRandomUseragent()
                ) { }

        public RequestData(string uri, EnumRequestMethod requestMethod) :
            this(
                new Uri(Uri.EscapeUriString(uri)), 
                requestMethod,
                EnumContentType.TEXT_HTML,
                GetRandomUseragent()
            ) { }
        
        public RequestData(string uri) :
           this(
               new Uri(Uri.EscapeUriString(uri)), 
               EnumRequestMethod.GET,
               EnumContentType.TEXT_HTML,
               GetRandomUseragent()
               ) { }

        public static string GetRandomUseragent()
        {
            AList<string> userAgents = new AList<string>();
            userAgents.Add("Mozilla/5.0 (iPad; CPU OS 7_2_1 like Mac OS X; en-US) AppleWebKit/531.43.4 (KHTML, like Gecko) Version/3.0.5 Mobile/8B112 Safari/6531.43.4");
            userAgents.Add("Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_8_7) AppleWebKit/5351 (KHTML, like Gecko) Chrome/37.0.881.0 Mobile Safari/5351");
            userAgents.Add("Mozilla/5.0 (Windows CE) AppleWebKit/5330 (KHTML, like Gecko) Chrome/37.0.896.0 Mobile Safari/5330");
            userAgents.Add("Mozilla/5.0 (compatible; MSIE 6.0; Windows NT 4.0; Trident/4.0)");
            userAgents.Add("Opera/8.57 (X11; Linux x86_64; en-US) Presto/2.10.234 Version/11.00");
            userAgents.Add("Mozilla/5.0 (X11; Linux i686) AppleWebKit/5331 (KHTML, like Gecko) Chrome/40.0.842.0 Mobile Safari/5331");
            userAgents.Add("Mozilla/5.0 (iPad; CPU OS 8_1_2 like Mac OS X; sl-SI) AppleWebKit/533.27.6 (KHTML, like Gecko) Version/3.0.5 Mobile/8B114 Safari/6533.27.6");
            userAgents.Add("Opera/9.63 (X11; Linux i686; sl-SI) Presto/2.12.215 Version/11.00");
            userAgents.Add("Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_8_9) AppleWebKit/5340 (KHTML, like Gecko) Chrome/40.0.816.0 Mobile Safari/5340");
            userAgents.Add("Mozilla/5.0 (iPhone; CPU iPhone OS 8_2_1 like Mac OS X; en-US) AppleWebKit/535.45.2 (KHTML, like Gecko) Version/3.0.5 Mobile/8B111 Safari/6535.45.2");
            userAgents.Add("Mozilla/5.0 (Windows NT 5.2) AppleWebKit/5362 (KHTML, like Gecko) Chrome/39.0.880.0 Mobile Safari/5362");
            userAgents.Add("Mozilla/5.0 (iPad; CPU OS 7_2_1 like Mac OS X; en-US) AppleWebKit/532.5.1 (KHTML, like Gecko) Version/3.0.5 Mobile/8B119 Safari/6532.5.1");
            userAgents.Add("Opera/8.23 (Windows NT 5.01; en-US) Presto/2.12.266 Version/12.00");
            userAgents.Add("Mozilla/5.0 (compatible; MSIE 9.0; Windows 98; Trident/5.1)");
            userAgents.Add("Opera/8.87 (Windows NT 6.1; sl-SI) Presto/2.11.204 Version/10.00");
            userAgents.Add("Mozilla/5.0 (Windows; U; Windows NT 6.1) AppleWebKit/534.9.5 (KHTML, like Gecko) Version/4.0.1 Safari/534.9.5");
            userAgents.Add("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 5.2; Trident/4.1)");
            userAgents.Add("Mozilla/5.0 (Windows NT 5.01; en-US; rv:1.9.1.20) Gecko/20140618 Firefox/35.0");
            userAgents.Add("Opera/9.64 (Windows 98; Win 9x 4.90; en-US) Presto/2.10.219 Version/12.00");
            userAgents.Add("Mozilla/5.0 (iPhone; CPU iPhone OS 7_0_2 like Mac OS X; en-US) AppleWebKit/535.7.5 (KHTML, like Gecko) Version/4.0.5 Mobile/8B113 Safari/6535.7.5");
            userAgents.Add("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/5330 (KHTML, like Gecko) Chrome/37.0.885.0 Mobile Safari/5330");
            userAgents.Add("Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_8_2 rv:3.0; sl-SI) AppleWebKit/534.17.2 (KHTML, like Gecko) Version/5.1 Safari/534.17.2");
            userAgents.Add("Mozilla/5.0 (Macintosh; PPC Mac OS X 10_6_0 rv:3.0) Gecko/20120102 Firefox/37.0");

            return userAgents.Get(new Random().Next(0, userAgents.Length));
        }

        public void AddAuthMethod(Auth auth)
        {
            this.Header.Add("Authorization", new AuthMethodHolder(auth).GetAuthData());
        }

        public void AddFormData(AList<FormKeypair> formKeyPair)
        {
            FormDataHolder formDataHolder = new FormDataHolder();
            formDataHolder.AddKeyPairs(formKeyPair);

            this._contentType.Set(EnumContentType.APPLICATION_FORM_URLENCODED);
            this._formDataHolder = formDataHolder;
        }
        
        public void AddMultipartFormData(AList<MultipartElement> multipartElements)
        {
            MultipartFormHolder multipartFormHolder = new MultipartFormHolder();

            for (int i = 0; i < multipartElements.Length; i++)
            {
                MultipartElement element = multipartElements.Get(i);
                
                if (!(element.Data is string || element.Data is AFileObject))
                    continue;
                
                multipartFormHolder.AddElement(element);
            }

            this._contentType.Set(EnumContentType.MULTIPART_FORMDATA);
            this._multipartFormHolder = multipartFormHolder;
        }

        public void AddContent(string content)
        {
            if (this._acceptTypeHolder.Contains(EnumCharsetType.UTF8))
            {
                this._content = Encoding.UTF8.GetBytes(content);
            }
            else
            {
                this._content = Encoding.Default.GetBytes(content);
            }

            this._requestMethod = EnumRequestMethod.POST;
        }

        public void SetContentType(EnumContentType contentType)
        {
            this._contentType.Set(contentType);
        }
        
        public void SetAccept(EnumCharsetType charsetType)
        {
            this._acceptTypeHolder.SetCharSet(charsetType);
        }

        public CookieContainer CookieContainer
        {
            get { return this._cookieContainer; }
            set { this._cookieContainer = value; }
        }

        public EnumRequestMethod RequestMethod
        {
            get { return this._requestMethod; }
            set { this._requestMethod = value; }
        }

        public byte[] Content
        {
            get
            {
                switch (_contentType.ContentTypeAsEnum)
                {
                    case EnumContentType.APPLICATION_FORM_URLENCODED:
                        return Encoding.UTF8.GetBytes(this._formDataHolder.GetKeyPairs());

                    case EnumContentType.MULTIPART_FORMDATA:
                    {
                        byte[] content = this._multipartFormHolder.GenerateData();

                        this._contentType.Set(EnumContentType.MULTIPART_FORMDATA,
                            this._multipartFormHolder.BoundaryContentType);

                        return content;
                    }
                    
                    default: return this._content;
                }
                
            }
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

        public ContentTypeHolder ContentTypeHolder
        {
            get => _contentType;
        }

        public AcceptTypeHolder AcceptTypeHolder
        {
            get => _acceptTypeHolder;
        }
    }
}
