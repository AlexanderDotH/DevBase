using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Web.RequestData;
using DevBaseData;
using Newtonsoft.Json;
using DevBaseServices.Mailcow;
using DevBaseServices.Mailcow.Requests;
using DevBase.Utilities;

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
            //DevBaseData.DataGenerator generator = new DevBaseData.DataGenerator(1000, 10, new DevBaseData.DataType[] { DevBaseData.DataType.Email, DataType.Password }, true);

            //foreach (string item in generator.GeneratedData)
            //{
            //    Console.WriteLine(item);
            //}

            //Console.ReadKey();

            MailcowService ms = new MailcowService(new Uri("https://mail.einfacheinalex.eu"), "B90DDF-D23A3D-576F65-2DC396-670507");

            string passwd = StringUtils.RandomString(64);

            CreateMailBoxObject cmbo = new CreateMailBoxObject
            {
                active = "1",
                domain = "einfacheinalex.eu",
                local_part = "testmail",
                name = "test",
                password = passwd,
                password2 = passwd,
                quota = "10",
                force_pw_update = "1",
                tls_enforce_in = "1",
                tls_enforce_out = "1"
            };
            CreateMailBox createMailBox = new CreateMailBox(cmbo);

            Console.WriteLine(ms.SendApiRequest(createMailBox));
            Console.ReadKey();
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
