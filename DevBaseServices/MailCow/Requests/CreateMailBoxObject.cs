using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace DevBaseServices.Mailcow.Requests
{
    public class CreateMailBoxObject
    {
        public string active { get; set; }
        public string domain { get; set; }
        public string local_part { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string password2 { get; set; }
        public string quota { get; set; }
        public string force_pw_update { get; set; }
        public string tls_enforce_in { get; set; }
        public string tls_enforce_out { get; set; }
    }
}
