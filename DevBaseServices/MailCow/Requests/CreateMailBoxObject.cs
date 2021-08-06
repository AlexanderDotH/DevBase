using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseServices.MailCow.Requests
{
    class CreateMailBoxObject
    {
        public bool active;
        public string domain;
        public string local_part;
        public string name;
        public string password;
        public string password2;
        public int quota;
        public bool force_pw_update;
        public bool tls_enforce_in;
        public bool tls_enforce_out;
    }
}
