using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using DevBase.Enums;
using DevBase.Web;
using DevBase.Web.RequestData;
using DevBase.Web.RequestData.Data;
using DevBase.Web.RequestData.Types;
using DevBase.Api.Apis.Deezer;
using DevBase.Api.Apis.Deezer.Structure.Json;
using DevBase.Generics;
using DevBase.IO;
using Newtonsoft.Json;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestData requestData = new RequestData("https://webhook.site/f060c671-c87d-4ba6-b039-b0bb9b920a19",
                EnumRequestMethod.POST);

            AList<MultipartElement> multipartElements = new AList<MultipartElement>();
            multipartElements.Add(new MultipartElement("id", "456456465546"));
            multipartElements.Add(new MultipartElement("file", AFile.ReadFile("C:\\Users\\alex\\Desktop\\audio.mp3")));

            requestData.AddMultipartFormData(multipartElements);

            Request r = new Request(requestData);
            r.GetResponse();
            Console.WriteLine();
        }
    }
}