using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Web.RequestData;
using Newtonsoft.Json;

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
            DevBaseData.DataGenerator generator = new DevBaseData.DataGenerator(100, 10, new DevBaseData.DataType[] { DevBaseData.DataType.Email, DevBaseData.DataType.Password });

            foreach (string item in generator.GeneratedData)
            {
                Console.WriteLine(item);
            }

            Console.ReadKey();
            //Data d1 = new Data
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
