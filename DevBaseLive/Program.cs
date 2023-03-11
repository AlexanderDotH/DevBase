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
using DevBase.Typography;
using DevBase.Utilities;
using Newtonsoft.Json;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] strings = GenerateTrash();
            
            string searchFor = strings[50];

            TestDefault(strings, searchFor);
            TestAList(strings, searchFor);

            /*RequestData requestData = new RequestData("http://localhost/upload.php",
                EnumRequestMethod.POST);

            AList<MultipartElement> multipartElements = new AList<MultipartElement>();
            multipartElements.Add(new MultipartElement("id", "456456465546"));
            multipartElements.Add(new MultipartElement("file", AFile.ReadFile("C:\\Users\\alex\\Desktop\\audio.mp3")));

            requestData.AddMultipartFormData(multipartElements);

            Request r = new Request(requestData);
            
            Console.WriteLine(r.GetResponse().GetContentAsString());*/
        }

        public static void TestDefault(string[] objects, string find)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] == find)
                    sw.Stop();
            }            
            
            Console.WriteLine("Default method found it in: " + sw.ElapsedTicks);
        }
        
        public static void TestAList(string[] objects, string find)
        {
            AList<string> objs = new AList<string>(objects);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (objs.FindEntry(find) == find)
                sw.Stop();
            
            Console.WriteLine("AList method found it in: " + sw.ElapsedMilliseconds);
        }

        private static string[] GenerateTrash()
        {
            AList<string> trash = new AList<string>();

            for (int i = 0; i < Math.Pow(10, 2); i++)
            {
                trash.Add(StringUtils.RandomString(100));
            }

            return trash.GetAsArray();
        }
    }
}