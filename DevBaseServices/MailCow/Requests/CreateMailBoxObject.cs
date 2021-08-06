using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

<<<<<<< HEAD
namespace DevBaseServices.Mailcow.Requests
{
    public class CreateMailBoxObject
=======
namespace DevBaseServices.MailCow.Requests
{
    class CreateMailBoxObject
>>>>>>> a61ac4cc39ea53ad39e806c8a1999a890eecc0dd
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
