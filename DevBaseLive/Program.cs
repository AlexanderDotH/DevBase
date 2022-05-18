using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.Web.RequestData;
using DevBaseData;
using DevBaseServices.Mailcow;
using DevBaseServices.Mailcow.Requests;
using DevBase.Utilities;
using DevBaseFormat;
using DevBaseFormat.Formats.LrcFormat;
using DevBaseFormat.Structure;

namespace DevBaseLive
{

    class Data
    {
        public string Neger { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //string response = new DevBase.Web.Request("https://music.xianqiao.wang/neteaseapiv2/lyric?id=24912403").GetResponse().GetContentAsString();
            //Console.WriteLine(response);
            //Console.ReadKey();

            List<string> list = new List<string>();
            list.Add("fenneg1");
            list.Add("fenneg2");
            list.Add("fenneg3");
            list.Add("fenneg4");

            GenericList<string> genericList = new GenericList<string>(list);

            Console.WriteLine("" +
                              "fenneg2" +
                              "");
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
