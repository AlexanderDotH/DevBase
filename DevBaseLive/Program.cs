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

using Newtonsoft.Json;

namespace DevBaseLive
{
   
        public class Album
    {
        [JsonProperty("cover")]
        public Cover cover { get; set; }

        [JsonProperty("__typename")]
        public string __typename { get; set; }
    }

    public class Cover
    {
        [JsonProperty("small")]
        public List<string> small { get; set; }

        [JsonProperty("medium")]
        public List<string> medium { get; set; }

        [JsonProperty("large")]
        public List<string> large { get; set; }

        [JsonProperty("explicitStatus")]
        public bool explicitStatus { get; set; }

        [JsonProperty("__typename")]
        public string __typename { get; set; }
    }

    public class Data
    {
        [JsonProperty("track")]
        public Track track { get; set; }
    }

    public class Extensions
    {
        [JsonProperty("cost")]
        public int cost { get; set; }
    }

    public class Lyrics
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("copyright")]
        public string copyright { get; set; }

        [JsonProperty("text")]
        public string text { get; set; }

        [JsonProperty("writers")]
        public string writers { get; set; }

        [JsonProperty("synchronizedLines")]
        public List<SynchronizedLine> synchronizedLines { get; set; }

        [JsonProperty("__typename")]
        public string __typename { get; set; }
    }

    public class Root
    {
        [JsonProperty("data")]
        public Data data { get; set; }

        [JsonProperty("extensions")]
        public Extensions extensions { get; set; }
    }

    public class SynchronizedLine
    {
        [JsonProperty("lrcTimestamp")]
        public string lrcTimestamp { get; set; }

        [JsonProperty("line")]
        public string line { get; set; }

        [JsonProperty("lineTranslated")]
        public object lineTranslated { get; set; }

        [JsonProperty("milliseconds")]
        public int milliseconds { get; set; }

        [JsonProperty("duration")]
        public int duration { get; set; }

        [JsonProperty("__typename")]
        public string __typename { get; set; }
    }

    public class Track
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("lyrics")]
        public Lyrics lyrics { get; set; }

        [JsonProperty("album")]
        public Album album { get; set; }

        [JsonProperty("__typename")]
        public string __typename { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Deezer deezerApi = new Deezer();

            /*var blockSize = 2048;
            var chunkSize = 6144;
            var decryptionChunkSize = 61440;

            string fileToDecrypt =
                "C:\\Users\\Alex\\Desktop\\6aaa3fec3468115d84b26c2e6184c7dd82dd9a041f24cbfe36a34698ac0abaa1c3a981e7908b8dcef85eeda7f79e2b2fbe618c49e3e6cf332f13752896dad73a59b3219b04c9f7e76e0d2b846f6def89.mp3";

            byte[] buffer = File.ReadAllBytes(fileToDecrypt);
                
            for (var n = 0; n < buffer.Length && n + blockSize < buffer.Length; n += chunkSize) {
                Console.WriteLine("From " + n + " - " + blockSize);
                //decryptChunk(new Uint8Array(buffer, n, blockSize));
            }*/
            var search = deezerApi.Search("photosynthese").GetAwaiter().GetResult();

            var data = deezerApi.DownloadSong(search.data[0].id.ToString()).GetAwaiter().GetResult();
            File.WriteAllBytes(search.data[0].title.ToString() + ".mp3", data);

            /*var n = deezerApi.GetUserData().GetAwaiter().GetResult();
            var d = deezerApi.GetSongDetails(search.data[0].id.ToString(), n.results.checkForm).GetAwaiter().GetResult();
            
            //Console.WriteLine(search.data[0].id.ToString());
            
            Console.WriteLine("ID: " + d.results.DATA.SNG_ID);
            Console.WriteLine("MediaVersion: " + d.results.DATA.MEDIA_VERSION);
            Console.WriteLine("MediaVersion: " + d.results.ISRC);

            
            var source = deezerApi.GetSongUrls(d.results.DATA.TRACK_TOKEN, n.results.USER.OPTIONS.license_token).GetAwaiter()
                .GetResult();

            foreach (var data in source.data)
            {
                foreach (var media in data.media)
                {
                    foreach (var s in media.sources)
                    {
                        Request request = new Request(s.url);
                        File.WriteAllBytes(d.results.DATA.SNG_ID, request.GetResponse().Content);
                        
                        Console.WriteLine(s.url);
                        //new WebClient().DownloadFile(s.url, "C:\\Users\\Alex\\Desktop\\test.mp3");
                        return;
                    }
                }
            }*/

            /*foreach (var media in source.media)
            {
                foreach (var format in media.formats)
                {
                    Console.WriteLine(format.);
                }
            }*/

            /*var res = deezerApi.Search("Live And Die").GetAwaiter().GetResult();

            foreach (var jsonDeezerSearchDataResponse in res.data)
            {
                var lyrics = deezerApi.GetLyrics(Convert.ToString(jsonDeezerSearchDataResponse.id)).GetAwaiter().GetResult();
            
                if (lyrics.data.track.lyrics == null)
                    continue;
                
                if (lyrics.data.track.lyrics.synchronizedLines == null)
                    continue;
                
                foreach (var line in lyrics.data.track.lyrics.synchronizedLines)
                {
                    Console.WriteLine(line.lrcTimestamp + "" + line.line);
                }
            }*/

            /*JsonDeezerJwtToken token = deezerApi.GetJwtToken().GetAwaiter().GetResult();
            
            RequestData requestData = new RequestData("https://pipe.deezer.com/api", EnumRequestMethod.POST);
            
            requestData.AddContent(@"{
            ""query"": ""query SynchronizedTrackLyrics($trackId: String!) {  track(trackId: $trackId) {    ...SynchronizedTrackLyrics    __typename  }}fragment SynchronizedTrackLyrics on Track {  id  lyrics {    ...Lyrics    __typename  }  album {    cover {      small: urls(pictureRequest: {width: 100, height: 100})      medium: urls(pictureRequest: {width: 264, height: 264})      large: urls(pictureRequest: {width: 800, height: 800})      explicitStatus      __typename    }    __typename  }  __typename}fragment Lyrics on Lyrics {  id  copyright  text  writers  synchronizedLines {    ...LyricsSynchronizedLines    __typename  }  __typename}fragment LyricsSynchronizedLines on LyricsSynchronizedLine {  lrcTimestamp  line  lineTranslated  milliseconds  duration  __typename}"",
            ""variables"": {
                ""trackId"": ""1762856547""
            }
        }");
            requestData.SetContentType(EnumContentType.APPLICATION_JSON);
            
            requestData.AddAuthMethod(new Auth(token.jwt, EnumAuthType.OAUTH2));

            Request request = new Request(requestData);

            string res = request.GetResponse().GetContentAsString();
            Console.WriteLine(res);*/

            /*Deezer deezerApi = new Deezer();

            JsonDeezerJwtToken token = deezerApi.GetJwtToken().GetAwaiter().GetResult();
            Console.WriteLine(token.jwt);

            /*Inputs? inputs = new Inputs();
            inputs.TryAdd("trackId", "1927741207");
            #1#

            RequestData data = new RequestData(" 	https://webhook.site/9c231d97-afd5-44d3-9120-33d5889ea2bb");

            data.RequestMethod = EnumRequestMethod.POST;

            data.Accept = "#1#*";
            data.Header.Add("Accept-Encoding", "gzip, deflate, br");
            //data.Header.Add("Content-Type", "application/json");
            
            data.Content = Encoding.UTF8.GetBytes(@"{
  ""query"": ""query SynchronizedTrackLyrics($trackId: String!) {  track(trackId: $trackId) {    ...SynchronizedTrackLyrics    __typename  }}fragment SynchronizedTrackLyrics on Track {  id  lyrics {    ...Lyrics    __typename  }  album {    cover {      small: urls(pictureRequest: {width: 100, height: 100})      medium: urls(pictureRequest: {width: 264, height: 264})      large: urls(pictureRequest: {width: 800, height: 800})      explicitStatus      __typename    }    __typename  }  __typename}fragment Lyrics on Lyrics {  id  copyright  text  writers  synchronizedLines {    ...LyricsSynchronizedLines    __typename  }  __typename}fragment LyricsSynchronizedLines on LyricsSynchronizedLine {  lrcTimestamp  line  lineTranslated  milliseconds  duration  __typename}"",
  ""variables"": {
    ""trackId"": ""1762856547""
  }
}");
            
            data.AddAuthMethod(new Auth(token.jwt, EnumAuthType.OAUTH2));

            Request request = new Request(data);

            string response = request.GetResponse().GetContentAsString();
            Console.WriteLine(response);
            */

            /*// To use NewtonsoftJsonSerializer, add a reference to NuGet package GraphQL.Client.Serializer.Newtonsoft
            var graphQLClient = new GraphQLHttpClient("https://pipe.deezer.com/api", new NewtonsoftJsonSerializer());

            var lyrics = new GraphQLRequest {
                Query =@"query SynchronizedTrackLyrics($trackId: String!) {  track(trackId: $trackId) {    ...SynchronizedTrackLyrics    __typename  }}fragment SynchronizedTrackLyrics on Track {  id  lyrics {    ...Lyrics    __typename  }  album {    cover {      small: urls(pictureRequest: {width: 100, height: 100})      medium: urls(pictureRequest: {width: 264, height: 264})      large: urls(pictureRequest: {width: 800, height: 800})      explicitStatus      __typename    }    __typename  }  __typename}fragment Lyrics on Lyrics {  id  copyright  text  writers  synchronizedLines {    ...LyricsSynchronizedLines    __typename  }  __typename}fragment LyricsSynchronizedLines on LyricsSynchronizedLine {  lrcTimestamp  line  lineTranslated  milliseconds  duration  __typename}",
                OperationName = "SynchronizedTrackLyrics",
                Variables = new {
                    trackId = "1762856547"
                },
                
            };
            
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.jwt);
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("Cookie", "_abck=CF1B0048C5324B20771F59D3F9E18B4B~-1~YAAQtCV+aJJ9dLaFAQAAGUpNugkKIFqyE5Nv0PEAesj+bOK/Uw0kKye3Y1r7QPdKWSIppkL4+akeLjAkcw+L6reiiDfQ4nEBbZlk2ensSga7O38KbruE3N6YnsA/NvlKzqT6AVsF3Tfd6A5iruSpQfkdMLttgTnqWXDcszae6Cx/EFNMzsNXqn691u6hqp1jdSPnrpMwf9xYz7/zW18DLcTomldEDuL2pLGDedGDpPgOkYXIarPZjK8ujW/s9TWnG6SiUeI6NKBgp3Y6t3XwDwC9IsAxnA2NiYbOit6Rc8XwuQCmCNe4U2s5TvzeTiEvt8iXXLWEM9CNIcd9JnJMmUUD1VD+XMDlMc+f64IddbWzjbFn6a2p119+28tOHFix~-1~-1~-1; ak_bmsc=D3701EF970730E57759FE039626AD027~000000000000000000000000000000~YAAQ26EkF3QrNbmFAQAAiCUnuhInnCANi/L4NG+zqE23yrZG3Zfuj1BpIJXWdosHedREwjfvPAcF4JoQtX6alSYL7RngK3/t1dllvthZSNsO7UxWDcIr2DShteBCdZi+ddQ9N04i3uwq2RHaBpIZ11aSzBECTwTBaPIFfbXmKQP7pSasivqdHQmKb+52Q/i3Ri9Ekgs237Q6sOdkPSapnKSyeJ1dtOU4qXC/SWKixMMQpbbcBsICpSp75za0+iRsv/Rp4u6N3sg7ny+kHvLu7ipnj/9OKxrDjjp1dLWvrr9NbeqwFi1U3nu1d2jtBYh43sDGhG2RayegZhBv2VnUJB3FHYYtdgys00ltp62zEW+XYfdY/FwVGAiXXKQ=; bm_sv=55A0D5DEBC1387BE0D8B1D2F2B29DD7B~YAAQtCV+aJN9dLaFAQAAGUpNuhKoISyJ4A3FR3RM4Zsct4TzS9r15Xb+NPru37kECEbBRanDzM0dqDK7D9N6KhTP79kIXZcEx00OJQKrnyExJLItOifovjOQmfWwF5Dfh6Cg0+bXu3aTPil1EmvGBbJYP3ESra5AZruXKitymVpAyipEbIT6U2Y6k1QsczxASImszpsRVyd3C9u4po0exzD4+CCKRtiJP7bvebYo0AquHP8sKlDj0Qnl8zAIM3Z8~1; bm_sz=D042D9373313B85509B9C2DAB94E62A0~YAAQ26EkF2q/KbmFAQAAeQX6uRK8Zs7RyB9wfln1f77881ylN8UNeRCHcLAZ7KlfuZgLD5+7DnCqkuETDoB+ngkNpajAwV98XsnPkmHJTxzWxXDM6p+TgrT02F9OiJXTpc6vApQcPBIybJmiWH3YjzuXBpOwnrodbuj4oU4YlbjPmx3Ln0+g4p05k7gAakOZg8VgvEtKHnwEYOhCDNg86j2j0iruddi3tRGwY3dKKsIY7LQacZG7yh3s0noBTXu4LPza+VulneaBZ+ziCsXb80bk2mrBHRtSVPVukRVbv240BCk=~3556150~4535350; dzr_uniq_id=dzr_uniq_id_fr08150b1ef44379f608020a8533418193af20a5");

            var graphQLResponse = graphQLClient.SendQueryAsync<Root>(lyrics).GetAwaiter().GetResult();
            var n = graphQLResponse.Data;
            
           
            Console.WriteLine(n.data.track);*/

            /*foreach (var lyricsSynchronizedLine in graphQLResponse.Data.data.track.lyrics.synchronizedLines)
            {
                Console.WriteLine(lyricsSynchronizedLine.line);
            }*/

            /*RequestData requestData = new RequestData("https://auth.deezer.com/login/arl?i=c&jo=p&rto=n");

            CookieContainer container = new CookieContainer();

            /*
            Cookie _abck = new Cookie("_abck",
                "CF1B0048C5324B20771F59D3F9E18B4B~-1~YAAQxKEkFyy15j2FAQAArofvpwnTJ3iUh70dtNjCtFRWErJffd5TeA6unP/rGERYkUR/1vpaN6tk9AtMrxOSKFJ1uKEWSre/98PulEgZodQ/2uztAU1f/vl4hYLMRD2Le98zmIZeKKKKUi3X+eevNoZxTgvhm2zue/bnYO7ksq/ojlS7JZGC3zH5o9cNW25PxUT0h7jsSPlRnK1XpT1sEkCHNJfG0VUhcLVSFjOV/f9y3V9FBXgn/xkojyH2AHER0N3OtczFSvgtuljGkD1fK4C6g1ABNGonAwCtHqlSPnFEFdAkEbjrUew8X1fJwbDGZZ8CvbVaSRpY7lHsHVtpGO0L35K9ehEIlPvIinlCS9l2kLykgx4OeMfjWP0Wt2oYgxEPDdnuNVLC9T/irGQp2pkUIUBAwbiJcA==~0~-1~-1", "/", "deezer.com");
            Cookie ak_bmsc = new Cookie("ak_bmsc", "35D9F773CDA29199DE589EC86AC9E1DA~000000000000000000000000000000~YAAQxKEkFy215j2FAQAArofvpxIkQb6riXRFEf+G+SmXNS9XE7HIXTsJGS0ca3Q8WeJ3MhZ/kvFILHCXHASD/ikNsq8JLAVZk/bgym1LCLsPC9gZS4R9APAzCPkcOsOXnVckJHuZNKwlPlqectceIFzAwrXyCm2neOG4Dq/aGTEQNZAlS3p3i5VJv7EhAR1+4qudBieSLO3nWCdzwOpPVk6UwUN2grybAAkqV7acIQsEtKEWTmzF+/lZ6IZ/SnYGx0w7jkr9Sw6hz9adtcT+SRFkMPbhq+PEFG0u6nEGyUvOWDjC3U+IQVFF598SkBfA29DzlILVan6si78xL7Y8dh0Q/MsXgKl7fRchXhk+OmOB5mp/KSy/feMtrDuydWseUl5DVk33baZwOKPm8w==", "/", "deezer.com");
            Cookie bm_sv = new Cookie("bm_sv","831A64C0499197289F12B77E5602E689~YAAQxKEkFy615j2FAQAArofvpxJ7086oQeZNY4RtDQEb6Zuil6+A/XrmmHWzQ8M1MBPPhv1t66uARsEmxwYOqY1U2lpGfQtNmf2RsWY9sGYHVsyiPppzYLJ79M6+QNr/2YC/IaRxI7bvqef8BlGGauIMieQ2eDmGfJLJjgCt/Ft2Xw4rxWRE6dq+1mvWnutg78XkOCgfNPSJ3liXlk1ka24BpmA+P8Tt2H2hkfvYDfwdvGvsCSrneqjF3AVm27ZUiw==~1", "/", "deezer.com");
            Cookie bm_sz = new Cookie("bm_sz","E9E8ABCE7E3A468BF3D501958F63D80E~YAAQLQcXAv8usX+FAQAA3cyupxLchMl9KZCJev2FtdSjiFsvrpmRi8Ul1fdeLvP5/Osq0i7sDHSPLsnzYdUV7Sxxy6fk8EYxeZjr76BVfh3o/kn+j/8/TZBarPt0kBwYSOa8LrB5MB3PDjWuksyZ0aAizHyLjuYfcty6wSrTtdFMwVXPm85xMthVPSqb7cRSyo8GCy3Ne8q73LAZut7Z/t2rYTCAbUS+zHrSKAH8POQ/JYLtFMn12e1VwkqqAEo8LSSJnanCC5sHUHRMSXxHVb1hqB+3Qz51NB2vYHYZD3EjSFs=~3622210~4605237", "/", "deezer.com");
            Cookie dzr_uniq_id = new Cookie("dzr_uniq_id","dzr_uniq_id_fr08150b1ef44379f608020a8533418193af20a5", "/", "deezer.com");

            container.Add(_abck);
            container.Add(ak_bmsc);
            container.Add(bm_sv);
            container.Add(bm_sz);
            container.Add(dzr_uniq_id);

            requestData.CookieContainer = container;#1#
            requestData.RequestMethod = EnumRequestMethod.POST;

            /*requestData.ContentType = new[] { EnumContentType.HTML };
            requestData.EncodingTypes = new[] { EnumEncodingType.UTF8 };#1#
            
            requestData.Header.Add("Accept", "#1#*");
            requestData.Header.Add("Accept-Encoding", "gzip, deflate, br");
            requestData.Header.Add("Cookie", "dzr_uniq_id=dzr_uniq_id_frbc23e096510b536bbd344475e7c61c87c05d94; _abck=CF1B0048C5324B20771F59D3F9E18B4B~0~YAAQn9AXAqVAFVuFAQAAvQGvpwnwTyfZG6KLfAr0OC19zlmJ8TudSo1kTpXETra15CK/sV78AR0pSs/rHqgwaMMvEa687zsiEzqQlM9/EVObBlT33f9UzKEg7JuVtE2KFL487gT5akMFdBwY+igP9ys7Ly+x2uC4gBL2w4nCjwOOXDerebdv+8SlvDvtDYSGZ1O+gMVUpKAz7V96JSb937MlYaxdigyL7+63WPfehArj/pNH0sb9GOePB97g6Czxua5BFi0okJNJOEY97lRe3QNmxAgZzb/CqBh2oRNmpT4v0MNvqYe4vclHS/rVW4Bbp/cMvj7t4rF+4YPVurq8tfPltK12A1/6BBBSpRUuU+WMDGcUxsSoRP8aZyW4oZ6ex0U1nTrm7SKfsamxdvWViag0LwChcHknBA==~-1~-1~-1; arl=447cae8221e4ba86cf8b479dae0d93fc3e70addfb4aed8a74cef16a799bfef493f22594d7118f34bbf65be490f32c0e77053bc323558d9b74e450b293174004539cfe43f6efb584dbdd59d038b6d0c803cc8db2fbe911290bdd0f0f2a80fbf89; comeback=1; ab.storage.deviceId.5ba97124-1b79-4acc-86b7-9547bc58cb18=%7B%22g%22%3A%22535ae124-06cc-5d7c-4f55-a40b08e12a2f%22%2C%22c%22%3A1662576184591%2C%22l%22%3A1673556074398%7D; ab.storage.userId.5ba97124-1b79-4acc-86b7-9547bc58cb18=%7B%22g%22%3A%225111209242%22%2C%22c%22%3A1662576184595%2C%22l%22%3A1673556074399%7D; ab.storage.sessionId.5ba97124-1b79-4acc-86b7-9547bc58cb18=%7B%22g%22%3A%229f53fd13-ba8b-90e5-a22a-f7ae771c54f9%22%2C%22e%22%3A1673559265157%2C%22c%22%3A1673556074398%2C%22l%22%3A1673557465157%7D; ak_bmsc=35D9F773CDA29199DE589EC86AC9E1DA~000000000000000000000000000000~YAAQn9AXAqZAFVuFAQAAvQGvpxLj2QJohYhosRko30MNWlpT7BpIbyT4DoojphXyyYKIcCkxZQmhicfwvwO4I40ZAan9iw51oZkC/F2cWmZ796nL/L0FIvKLkYvQvE4wsuKzCwwTYeSrMSZY5v40JHL+rrQsAF+GNPUTVj4aINqLrl3DBz84jja18Y6U0W1X/k4x1t1jo7WFOn7rVz8AMLAIS1GNw1csHoMrSniwbcj6mcKnSQk/yAp3fB9yuMcP0ne7SUonswF8AqfG58BiuP84tDYqHS8pQLqr0lDntvmQzjub2+Ge0DGSFzPN6lTAidM2+X5tf7726yQJTIdbeD/T4WjxksdGREeFHbYSZgvHL+8n59kb4VNkJPsu+4aiIg6GflzCNKWDhQ==; bm_sz=57CA358B5373057BF707E0AF3E125849~YAAQn9AXAqdAFVuFAQAAvQGvpxIqG0C29+2L+tsuEFeWjd+QBSOeinEqnN4n9A5ebZyQyIVeb5w0AdcP9jx3ch2KF1nNGuVEG9rZNopVgjjmyI7tPJAaVeiMSR1a+yJkl9gJh+fIz14ruNzDgefsVWCf9pCdQmbzF1Evzm/IsoJxZS/avzFWlz6AwPgjmbc05XBNgcSkL9m3arkh/CiMgCxN1y6J8V9tQMj2gZRwaaeK2VS0iLX4e1AS8GdL/2m2QzgdmVefnRfPiQLYi0rOxiIXDbnGuQdBe02VOjx9rQD2ipA=~3621957~3359812; bm_sv=831A64C0499197289F12B77E5602E689~YAAQiNAXAoglcCGFAQAAOAjjpxJSp+p8+ytP2d7xKottbNoKdCXCGp9mRrAeQSSTDd+nH/OWAG09Ox3NbXXQO05PpVpeBYqMe/rN7QEJxD8buiXJAvBViwAu88GvPAmJacJAJM1jaVYZbftV9SNZNZOqHGEsKgC4s7H6+AX8y6dY+hqAKnCa+v67ANO1ItrFoHZ2DOcyuP7xkohkPWZv0J2ZIW5sxV8N2eM/UDlgvG9YmOU48wN421ZQW1Ifal8lXA==~1; sid=frf0a902aabfd798fda01ac31052590d5033170d; bm_mi=777177B1A96261681750DF8015BBCB0A~YAAQiNAXAkvhbCGFAQAAyk3MpxKhX6Bq+BSjJ+xJncyX/CJbtG9Xx41ep36zPZhLlhTnWP9A61ZSl/Kn2pFQ0+7V5ngfXtVt2EBN+AcQnRjPbAhPo+EeEEccHRbIl3PRrU7bEH/w6wMNE8Ry1gK7IDAImPCaB6VMgQgSrPoAm2BWE93QN2uh2rc4OSzBQOIKstNmMoZl4tCiYh6kcSRKcc4Ybjd5NcBfpjEuYc11Je7oDb9e3PnCTK7pjoPbZFqLS+fFREF5iPBelxV65Zpz2a8sayxFVAm0VDiQv0noA7eN662eDCs7+A6iZaLCqWha94XiTkMh2gYv2GWNYxyBpDA3IfFH~1; consentStatistics=1; consentMarketing=1");
            
            Request r = new Request(requestData);

            string n = r.GetResponse().GetContentAsString();
            Console.WriteLine(n);*/

            /*Console.WriteLine("Hello World!");

            int testCount = 100000000;
            long tryOne = 0;

            for (int i = 0; i < testCount; i++)
            {
                tryOne += TestOne();
            }

            double avgTestOne = tryOne;
            Console.WriteLine("Avg 1: " + avgTestOne);
            
            //_----
            
            long tryTwo = 0;

            for (int i = 0; i < testCount; i++)
            {
                tryTwo += TestTwo();
            }

            double avgTestTwo = tryTwo;
            Console.WriteLine("Avg 2: " + avgTestTwo);
            */

        }

        static long TestOne()
        {
            string[] array = new[] { "A", "B", "C", "D" };
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            array = CopyToAdd(array, "E");
            
            sw.Stop();
            return sw.ElapsedTicks;
        }
        
        static long TestTwo()
        {
            string[] array = new[] { "A", "B", "C", "D" };
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            array = ResizeAdd(array, "E");
            
            sw.Stop();
            return sw.ElapsedTicks;
        }

        static string[] CopyToAdd(string[] array, string value)
        {

            string[] newArray = new string[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = value;

            if (newArray.Length % 50 == 0)
            {
                GCHandle.Alloc(newArray, GCHandleType.Normal).Free();
            }
            
            return newArray;
        }
        
        static string[] ResizeAdd(string[] array, string value)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
            return array;
        }
    }
}