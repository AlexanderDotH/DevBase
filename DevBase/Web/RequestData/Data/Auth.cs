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
            _token = token;
            _authType = authType;
        }

        public string Token
        {
            get => _token;
            set => _token = value;
        }

        public EnumAuthType AuthType
        {
            get => _authType;
            set => _authType = value;
        }
    }
}
