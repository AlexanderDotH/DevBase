using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevBase.Enums;
using DevBase.Generic;
using DevBase.IO;
using DevBase.Web.RequestData;
using DevBaseData;
using DevBaseServices.Mailcow;
using DevBaseServices.Mailcow.Requests;
using DevBase.Utilities;
using DevBase.Web;
using DevBase.Web.RequestData.Data;
using DevBase.Web.ResponseData;
using DevBaseFormat;
using DevBaseFormat.Formats.LrcFormat;
using DevBaseFormat.Formats.MmlFormat;
using DevBaseFormat.Structure;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task.Factory.StartNew(async() =>
            //{
            //    try
            //    {
            //        string clientID = "zU4XHVVkc2tDPo4t";
            //        string clientSecret = "VJKhDFqJPqvsPVNBV6ukXTJmwlvbttP7wlMlrc72se4=";

            //        GenericList<FormKeypair> formData = new GenericList<FormKeypair>();
            //        formData.Add(new FormKeypair("client_id", clientID));
            //        formData.Add(new FormKeypair("scope", "r_usr+w_usr+w_sub"));

            //        RequestData requestData = new RequestData(new Uri("https://auth.tidal.com/v1/oauth2/device_authorization"),
            //            EnumRequestMethod.POST,
            //            new EnumContentType[] { EnumContentType.FORM },
            //            new EnumEncodingType[] { EnumEncodingType.UTF8 },
            //            formData);

            //        string authToken = Convert.ToBase64String(Encoding.Default.GetBytes(clientID + ":" + clientSecret));
            //        requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC));

            //        Request request = new Request(requestData);

            //        ResponseData response = await request.GetResponseAsync();

            //        Console.WriteLine(response.GetContentAsString());
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }

            //});

            //Console.WriteLine("reached");

            //Console.ReadKey();


            //var parser = new FileFormatParser<LrcObject>(new MmlParser<LrcObject>());
            //AFile.GetFiles("C:\\Users\\alexander.heuschkel\\source\\repos\\EinfachEinAlex\\DevBase\\DevBaseLive\\bin\\Debug").GetAsList().ForEach(t =>
            //{
            //    var parsed = parser.FormatFromFile(t.FileInfo.FullName);
            //    parsed.Lyrics.GetAsList().ForEach(l => Console.WriteLine(l.Line + ":" + l.TimeStamp));
            //});

            //Console.ReadKey();

            //string response = new DevBase.Web.Request("https://music.xianqiao.wang/neteaseapiv2/lyric?id=1472893983").GetResponse().GetContentAsString();
            //File.WriteAllText("out.txt", response);
            //Console.WriteLine(response);
            //Console.ReadKey();

            List<string> list = new List<string>();
            list.Add("fenneg1");
            list.Add("fenneg2");
            list.Add("fenneg3");
            list.Add("fenneg4");

            GenericList<string> genericList = new GenericList<string>(list);
            var f = genericList.GetRangeAsList(0, 4);

            genericList.Remove("fenneg3");

            genericList.GetAsList().ForEach(t => Console.WriteLine(t));

            Console.ReadKey();

            //Thread.Sleep(1000);
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //FileFormatParser<LrcObject> fileFormatParser =
            //    new FileFormatParser<LrcObject>(new LrcParser<LrcObject>());

            //LrcObject lrcObject = fileFormatParser.FormatFromFile("C:\\Users\\Alex\\Downloads\\fse.lrc");

            //Console.WriteLine(lrcObject.Artist);
            //Console.WriteLine(lrcObject.Album);
            //Console.WriteLine(lrcObject.Title);
            //Console.WriteLine(lrcObject.Author);
            //Console.WriteLine(lrcObject.By);
            //Console.WriteLine(lrcObject.Offset);
            //Console.WriteLine(lrcObject.Re);
            //Console.WriteLine(lrcObject.Version);

            //lrcObject.Lyrics.ForEach(t => Console.WriteLine(t.Line + ":" + t.TimeStamp));

            //Console.WriteLine(sw.ElapsedMilliseconds + "ms");

            //Console.ReadKey();

            //DevBaseData.DataGenerator generator = new DevBaseData.DataGenerator(1000, 10, new DevBaseData.DataType[] { DevBaseData.DataType.Email, DataType.Password }, true);

            //foreach (string item in generator.GeneratedData)
            //{
            //    Console.WriteLine(item);
            //}

            //Console.ReadKey();

            //MailcowService ms = new MailcowService(new Uri("https://mail.einfacheinalex.eu"), "B90DDF-D23A3D-576F65-2DC396-670507");

            //string passwd = StringUtils.RandomString(64);

            //CreateMailBoxObject cmbo = new CreateMailBoxObject
            //{
            //    active = "1",
            //    domain = "einfacheinalex.eu",
            //    local_part = "testmail",
            //    name = "test",
            //    password = passwd,
            //    password2 = passwd,
            //    quota = "10",
            //    force_pw_update = "1",
            //    tls_enforce_in = "1",
            //    tls_enforce_out = "1"
            //};
            //CreateMailBox createMailBox = new CreateMailBox(cmbo);

            //Console.WriteLine(ms.SendApiRequest(createMailBox));
            //Console.ReadKey();
            //Data d1 = new Datad
            //{
            //    Neger = "dawd"
            //};

            //string json = JsonConvert.SerializeObject(d1);

            //DevBase.Web.RequestData.RequestData data = new DevBase.Web.RequestData.RequestData(
            //    new Uri("https://webhook.site/5ab2f26b-7d1a-4b70-b179-a06d85bdd557"), 
            //    RequestMethod.POST, 
            //    ContentType.JSON, 
            //    json, "neger");

            //DevBase.Web.Request request = new DevBase.Web.Request(data);
            //Console.WriteLine(request.GetResponse().GetContentAsString());
            //Console.ReadKey();
        }
    }
}
