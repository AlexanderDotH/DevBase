﻿using DevBase.Web.RequestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DevBase.Enums;

namespace DevBaseServices.Mailcow
{
    public class MailcowService
    {
        private Uri _serverUri;
        private string _apiKey;

        public MailcowService(Uri serverUri, string apiKey)
        {
            this._serverUri = serverUri;
            this._apiKey = apiKey;
        }

        public string SendApiRequest(IServiceData serviceData)
        {
            RequestData requestData = new RequestData(
                new Uri(_serverUri + serviceData.EndpointDirectory()),
                EnumRequestMethod.POST,
                new EnumContentType[] { EnumContentType.JSON },
                new EnumEncodingType[] { EnumEncodingType.UTF8 },
                serviceData.RequestString());

            requestData.Header.Add("X-API-Key", this._apiKey);

            DevBase.Web.Request request = new DevBase.Web.Request(requestData);
            return Encoding.ASCII.GetString(request.GetResponse().Content);
        }
    }
}
