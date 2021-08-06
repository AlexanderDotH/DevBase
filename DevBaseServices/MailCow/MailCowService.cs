using DevBase.Web.RequestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

<<<<<<< HEAD
namespace DevBaseServices.Mailcow
{
    //Untested
    public class MailcowService
=======
namespace DevBaseServices.MailCow
{
    class MailCowService
>>>>>>> a61ac4cc39ea53ad39e806c8a1999a890eecc0dd
    {
        private Uri _serverUri;
        private string _apiKey;

<<<<<<< HEAD
        public MailcowService(Uri serverUri, string apiKey)
=======
        public MailCowService(Uri serverUri, string apiKey)
>>>>>>> a61ac4cc39ea53ad39e806c8a1999a890eecc0dd
        {
            this._serverUri = serverUri;
            this._apiKey = apiKey;
        }

        public string SendApiRequest(IServiceData serviceData)
        {
            RequestData requestData = new RequestData(
<<<<<<< HEAD
                new Uri(_serverUri + serviceData.EndpointDirectory()), 
=======
                new Uri(_serverUri, serviceData.EndpointDirectory(), false), 
>>>>>>> a61ac4cc39ea53ad39e806c8a1999a890eecc0dd
                RequestMethod.POST, 
                ContentType.JSON, 
                serviceData.RequestString());

            requestData.Header.Add("X-API-Key", this._apiKey);

            DevBase.Web.Request request = new DevBase.Web.Request(requestData);
            
            return Encoding.ASCII.GetString(request.GetResponse().Content);
        }
    }
}
