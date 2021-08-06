using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

<<<<<<< HEAD
namespace DevBaseServices.Mailcow.Requests
{
    public class CreateMailBox : IServiceData
    {
        public string EndpointDirectory() => "api/v1/add/mailbox";
=======
namespace DevBaseServices.MailCow.Requests
{
    class CreateMailBox : IServiceData
    {
        public string EndpointDirectory() => "/api/v1/add/mailbox";
>>>>>>> a61ac4cc39ea53ad39e806c8a1999a890eecc0dd

        private CreateMailBoxObject _createMailBoxObject;

        public CreateMailBox(CreateMailBoxObject createMailBoxObject)
        {
            this._createMailBoxObject = createMailBoxObject;
        }

        public string RequestString()
        {
            return JsonSerializer.Serialize(_createMailBoxObject);
        }
    }
}
