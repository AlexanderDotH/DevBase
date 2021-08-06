using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DevBaseServices.Mailcow.Requests
{
    public class CreateMailBox : IServiceData
    {
        public string EndpointDirectory() => "api/v1/add/mailbox";

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
