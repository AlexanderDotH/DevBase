using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DevBase.Web.ResponseData
{
    public class ResponseData
    {
        private byte[] _content;
        private HttpStatusCode _httpStatusCode;
        private HttpWebResponse _response;

        public ResponseData(HttpWebResponse response, byte[] content, HttpStatusCode httpStatusCode)
        {
            this._content = content;
            this._httpStatusCode = httpStatusCode;
            this._response = response;
        }

        public ResponseData(HttpWebResponse response, string content, HttpStatusCode httpStatusCode) : this(response, Encoding.Default.GetBytes(content), httpStatusCode) { }

        public HttpWebResponse Response
        {
            get { return this._response; }
            set { this._response = value; }
        }

        public string GetContentAsString()
        {
            Encoding encoding = Encoding.GetEncoding(this._response.CharacterSet);
            return encoding.GetString(this._content);
        }

        public byte[] Content
        {
            get { return this._content; }
        }

        public HttpStatusCode StatusCode
        {
            get { return this._httpStatusCode; }
        }
    }
}
