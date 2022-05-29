using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBase.Web.RequestData.Data
{
    public class FormKeypair
    {
        private string _key;
        private string _value;

        public FormKeypair(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public string Key
        {
            get => _key;
            set => _key = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
