using System.Diagnostics;
using System.Text;
using DevBase.Cryptography.BouncyCastle.ECDH;
using DevBase.Cryptography.BouncyCastle.Extensions;
using DevBase.Cryptography.BouncyCastle.Identifier;
using DevBase.Cryptography.BouncyCastle.Random;
using DevBase.Cryptography.BouncyCastle.Sealing;
using DevBase.Generics;
using DevBase.Utilities;
using Random = DevBase.Cryptography.BouncyCastle.Random.Random;

namespace DevBaseLive
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Deezer api = new Deezer();

            var search = api.Search("Midnight Sun");
            
            var lyrics = api.DownloadSong(search.GetAwaiter().GetResult().data[0].id.ToString()).GetAwaiter().GetResult();

            OpenAi openAi = new OpenAi("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");

            var result = openAi.Transcribe(lyrics).GetAwaiter().GetResult();*/

            string publicKey =
                "MIIBSzCCAQMGByqGSM49AgEwgfcCAQEwLAYHKoZIzj0BAQIhAP////8AAAABAAAAAAAAAAAAAAAA////////////////MFsEIP////8AAAABAAAAAAAAAAAAAAAA///////////////8BCBaxjXYqjqT57PrvVV2mIa8ZR0GsMxTsPY7zjw+J9JgSwMVAMSdNgiG5wSTamZ44ROdJreBn36QBEEEaxfR8uEsQkf4vOblY6RA8ncDfYEt6zOg9KE5RdiYwpZP40Li/hp/m47n60p8D54WK84zV2sxXs7LtkBoN79R9QIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNaiUU4I4vMYBm+PLZ3a5qC4z9mYj5n+KKx/+MrPJq37pUW87mH3zWcrGb2O+QW/zr+TIR4cQbeBaktjsQBkiSM=";

            string privateKey = "jsp6E432kv5WzUclbRQcgnVqrx3UstjKBxggK7Hwbjs=";

            Sealing client = new Sealing(publicKey);

            string message = "Hello my boy!!";

            byte[] seal = client.Seal(Encoding.ASCII.GetBytes(message));
            
            Debug.WriteLine($"Sealed {Convert.ToBase64String(seal)}");

            Sealing server = new Sealing(publicKey, privateKey);
            byte[] unsealed = server.UnSeal(seal);
            
            Debug.WriteLine($"UnSealed {Encoding.ASCII.GetString(unsealed)}");

            //----------------------------------------

            byte[] seal2 = server.Seal(Encoding.ASCII.GetBytes("Danke man du bist tooll!!"));
            
            Debug.WriteLine($"Sealed from Server {Convert.ToBase64String(seal2)}");

            byte[] unseal2 = client.UnSeal(seal2);
            Debug.WriteLine($"UnSealed {Encoding.ASCII.GetString(unseal2)}");



            /*var bob = new ECDHEngineBuilder().GenerateKeyPair();
            
            Debug.WriteLine($"PublicKey {Convert.ToBase64String(bob.PublicKey.PublicKeyToArray())}");
            Debug.WriteLine($"PrivateKey {Convert.ToBase64String(bob.PrivateKey.PrivateKeyToArray())}");*/


            /*var alice = new ECDHEngineBuilder().GenerateKeyPair();

            byte[] shared1 = bob.DeriveKeyPairs(alice.PublicKey);
            byte[] shared2 = alice.DeriveKeyPairs(bob.PublicKey);
            
            Debug.WriteLine(Convert.ToBase64String(alice.PublicKey.ToArray()));*/
            /*
            while (true)
            {
                Console.WriteLine(Identification.GenerateRandomId());
            }
            */

            /*OpenLyricsClient olc = new OpenLyricsClient();
            
            AuthorizationCodeRefreshResponse response = new OAuthClient().RequestToken(
                new AuthorizationCodeRefreshRequest("5506575c84334b25978bda35ee43e6fd", "af2957198b104760bdf4bb3a48915365", "AQBLp8AMBIkARE8yuaRoJ1I7BUql0-Z3K5d0HGt-Gj4s707XLZc9cADgqU9MSEtgUvkAfj404w-KSE4WPgcJS93QERaRl8TOnn7WD3rdeW9rj9FRlyA_yhK_tvBTn8HihDA")).GetAwaiter().GetResult();

            Console.WriteLine(response.AccessToken);*/

            /*var n = olc.GetAccessToken(
                    "AQBVPi3EZgLmii6Ty2OesIpJ5h02QiHtAro3Gvf1D5MI2XZXZZv3I_BRttQGXOAPrD8lJiFmaDEpBnywnV0oytSXkO3fUDzrnXcRvxnMSnXZC3Oel0nyCnkV8TqVfFW3JZg")
                .GetAwaiter().GetResult();*/

            //Console.WriteLine(olc.SubmitAiSync("nick", "joe", 0, "nick").GetAwaiter().GetResult().ID);
            //Console.WriteLine(olc.GetAiSyncResult("bd432c797574a1bea5e3ca96dawd54bb5032").GetAwaiter().GetResult().SRT);

            /*var _tokens = new AList<string>(
                "60ec6c9b8f5e6d6e9a89fc3bf78429ed77b1816a",
                "026a3e5c1fdea8452683b58d771a4938983e8e72",
                "4c2c19b8c3ed2d3b6094d91cda439ff4ee6c698f",
                "1605bd7745c968810952aeb112e74f82bf4c5448");

            var _replicate = new Replicate(_tokens);

            var pred = _replicate.Predict("23241e5731b44fcb5de68da8ebddae1ad97c5094d24f94ccb11f7c1d33d661e2",
                "https://audio.openlyricsclient.com/f5b11ddb97d7d590eed1ea284bfa5f17",
                "large-v2", "1605bd7745c968810952aeb112e74f82bf4c5448", "https://openlyricsclient.com/api/ai/webhook");

            Console.WriteLine(pred.GetAwaiter().GetResult().id);*/

            //ReplicatePredictionResponse resu = pred.GetAwaiter().GetResult();

            /*var nick = _replicate.GetResult("kdt2umzwezbxbigs7sknoisfhq", "60ec6c9b8f5e6d6e9a89fc3bf78429ed77b1816a").GetAwaiter().GetResult();
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