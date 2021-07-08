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
    
        public ResponseData(byte[] content, HttpStatusCode httpStatusCode)
        {
            this._content = content;
            this._httpStatusCode = httpStatusCode;
        }

        public ResponseData(string content, HttpStatusCode httpStatusCode) : this(Encoding.ASCII.GetBytes(content), httpStatusCode) { }

        public string GetContentAsString()
        {
            return Encoding.ASCII.GetString(this._content);
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
