using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DevBase.Utilities;

namespace DevBase.Web.ResponseData
{
    [Serializable]
    public class ResponseData
    {
        private byte[] _content;
        private HttpStatusCode _httpStatusCode;
        private HttpWebResponse _response;
        private Encoding _encoding;

        public ResponseData(HttpWebResponse response)
        {
            this._response = response;
            this._content = MemoryUtils.StreamToByteArray(response.GetResponseStream());
            this._encoding =  Encoding.GetEncoding(string.IsNullOrWhiteSpace(this._response.CharacterSet) ? "UTF-8" : this._response.CharacterSet);
            this._httpStatusCode = response.StatusCode;
        }
        
        public HttpWebResponse Response
        {
            get { return this._response; }
            set { this._response = value; }
        }

        public string GetContentAsString()
        {
            return this._encoding.GetString(this._content);
        }

        public byte[] Content
        {
            get { return this._content; }
        }

        public HttpStatusCode StatusCode
        {
            get { return this._httpStatusCode; }
        }

        public Encoding Encoding
        {
            get => this._encoding;
        }
    }
}
