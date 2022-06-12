using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Enums;
using DevBase.Generic;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;

namespace DevBaseAPI.MusixMatchAPI.Requests
{
    public class GenerateUserTokenRequest
    {

        private readonly string _baseUrl;

        public GenerateUserTokenRequest()
        {
            _baseUrl = "https://apic-desktop.musixmatch.com";
        }

        public string GenerateUserToken()
        {
            GenericList<FormKeypair> keyPairs = new GenericList<FormKeypair>();
            keyPairs.Add(new FormKeypair("format","json"));
            keyPairs.Add(new FormKeypair("app_id", "web-desktop-app-v1.0"));
            keyPairs.Add(new FormKeypair("usertoken", "issuenewtoken"));
            keyPairs.Add(new FormKeypair("guid", "json"));

            RequestData requestData = new RequestData(new Uri(_baseUrl), EnumRequestMethod.GET, new EnumContentType[]
            { EnumContentType.JSON }, new EnumEncodingType[] { EnumEncodingType.UTF8 }, keyPairs);

            Guid guid = Guid.NewGuid();


        }

    }
}
