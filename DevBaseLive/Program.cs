using System.Diagnostics;
using DevBase.Api.Apis.OpenLyricsClient;
using DevBase.Api.Apis.Replicate;
using DevBase.Api.Apis.Replicate.Structure;
using DevBase.Generics;
using DevBase.Utilities;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenLyricsClient olc = new OpenLyricsClient();
            var n = olc.GetAccessToken(
                    "AQBVPi3EZgLmii6Ty2OesIpJ5h02QiHtAro3Gvf1D5MI2XZXZZv3I_BRttQGXOAPrD8lJiFmaDEpBnywnV0oytSXkO3fUDzrnXcRvxnMSnXZC3Oel0nyCnkV8TqVfFW3JZg")
                .GetAwaiter().GetResult();
            
            Console.WriteLine(n.AccessToken);

            /*var _tokens = new AList<string>(
                "ab6d730d3629ff370f8b33b1daf19eb9147951d1",
                "caa411c67422912bead4cca0c94bf788c7e0a0f5",
                "60ec6c9b8f5e6d6e9a89fc3bf78429ed77b1816a",
                "026a3e5c1fdea8452683b58d771a4938983e8e72",
                "4c2c19b8c3ed2d3b6094d91cda439ff4ee6c698f",
                "1605bd7745c968810952aeb112e74f82bf4c5448");

            var _replicate = new Replicate(_tokens);

            /*var pred = _replicate.Predict("23241e5731b44fcb5de68da8ebddae1ad97c5094d24f94ccb11f7c1d33d661e2",
                "https://audio.openlyricsclient.com/62ef0ce531fbe33fffe4fd9z4574576de39a7b0f",
                "large-v2");#1#

            //ReplicatePredictionResponse resu = pred.GetAwaiter().GetResult();

            var nick = _replicate.GetResult("kdt2umzwezbxbigs7sknoisfhq", "60ec6c9b8f5e6d6e9a89fc3bf78429ed77b1816a").GetAwaiter().GetResult();
            Console.WriteLine(nick.output.transcription);*/

            //Console.WriteLine(pred.GetAwaiter().GetResult().id);

            /*string[] strings = GenerateTrash();
            
            string searchFor = strings[50];

            TestDefault(strings, searchFor);
            TestAList(strings, searchFor);*/

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