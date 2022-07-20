using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Enums;

namespace DevBase.Web.RequestData.Data
{
    public class Auth
    {
        private string _token;
        private EnumAuthType _authType;

        public Auth(string token, EnumAuthType authType)
        {
            this._token = token;
            this._authType = authType;
        }

        public string Token
        {
            get => this._token;
            set => this._token = value;
        }

        public EnumAuthType AuthType
        {
            get => this._authType;
            set => this._authType = value;
        }
    }
}
