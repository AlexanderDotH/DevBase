using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DevBase.Async.Task;
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
using DevBase.Web.WebCache;
using DevBaseFormat;
using DevBaseFormat.Formats.EnvFormat;
using DevBaseFormat.Formats.LrcFormat;
using DevBaseFormat.Formats.MmlFormat;
using DevBaseFormat.Structure;
using Newtonsoft.Json;

namespace DevBaseLive
{

    class Type1{}
    class Type2{}       
    class Type3{}

    public class PauseTokenSource
    {
        bool _paused = false;
        bool _pauseRequested = false;

        TaskCompletionSource<bool> _resumeRequestTcs;
        TaskCompletionSource<bool> _pauseConfirmationTcs;

        readonly SemaphoreSlim _stateAsyncLock = new SemaphoreSlim(1);
        readonly SemaphoreSlim _pauseRequestAsyncLock = new SemaphoreSlim(1);

        public async Task<bool> IsPaused(CancellationToken token = default(CancellationToken))
        {
            await _stateAsyncLock.WaitAsync(token);
            try
            {
                return _paused;
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        public async Task ResumeAsync(CancellationToken token = default(CancellationToken))
        {
            await _stateAsyncLock.WaitAsync(token);
            try
            {
                if (!_paused)
                {
                    return;
                }

                await _pauseRequestAsyncLock.WaitAsync(token);
                try
                {
                    var resumeRequestTcs = _resumeRequestTcs;
                    _paused = false;
                    _pauseRequested = false;
                    _resumeRequestTcs = null;
                    _pauseConfirmationTcs = null;
                    resumeRequestTcs.TrySetResult(true);
                }
                finally
                {
                    _pauseRequestAsyncLock.Release();
                }
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        public async Task PauseAsync(CancellationToken token = default(CancellationToken))
        {
            await _stateAsyncLock.WaitAsync(token);
            try
            {
                if (_paused)
                {
                    return;
                }

                Task pauseConfirmationTask = null;

                await _pauseRequestAsyncLock.WaitAsync(token);
                try
                {
                    _pauseRequested = true;
                    _resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _pauseConfirmationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    pauseConfirmationTask = WaitForPauseConfirmationAsync(token);
                }
                finally
                {
                    _pauseRequestAsyncLock.Release();
                }

                await pauseConfirmationTask;

                _paused = true;
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        private async Task WaitForResumeRequestAsync(CancellationToken token)
        {
            using (token.Register(() => _resumeRequestTcs.TrySetCanceled(), useSynchronizationContext: false))
            {
                await _resumeRequestTcs.Task;
            }
        }

        private async Task WaitForPauseConfirmationAsync(CancellationToken token)
        {
            using (token.Register(() => _pauseConfirmationTcs.TrySetCanceled(), useSynchronizationContext: false))
            {
                await _pauseConfirmationTcs.Task;
            }
        }

        internal async Task PauseIfRequestedAsync(CancellationToken token = default(CancellationToken))
        {
            Task resumeRequestTask = null;

            await _pauseRequestAsyncLock.WaitAsync(token);
            try
            {
                if (!_pauseRequested)
                {
                    return;
                }
                resumeRequestTask = WaitForResumeRequestAsync(token);
                _pauseConfirmationTcs.TrySetResult(true);
            }
            finally
            {
                _pauseRequestAsyncLock.Release();
            }

            await resumeRequestTask;
        }
    }

    public class JsonTidalAccountRefreshAccess
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("user")]
        public JsonTidalUser User { get; set; }
    }

    public class JsonTidalUser
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public object Email { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("fullName")]
        public object FullName { get; set; }

        [JsonProperty("firstName")]
        public object FirstName { get; set; }

        [JsonProperty("lastName")]
        public object LastName { get; set; }

        [JsonProperty("nickname")]
        public object Nickname { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("address")]
        public object Address { get; set; }

        [JsonProperty("city")]
        public object City { get; set; }

        [JsonProperty("postalcode")]
        public object Postalcode { get; set; }

        [JsonProperty("usState")]
        public object UsState { get; set; }

        [JsonProperty("phoneNumber")]
        public object PhoneNumber { get; set; }

        [JsonProperty("birthday")]
        public object Birthday { get; set; }

        [JsonProperty("gender")]
        public object Gender { get; set; }

        [JsonProperty("imageId")]
        public object ImageId { get; set; }

        [JsonProperty("channelId")]
        public int ChannelId { get; set; }

        [JsonProperty("parentId")]
        public int ParentId { get; set; }

        [JsonProperty("acceptedEULA")]
        public bool AcceptedEULA { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("facebookUid")]
        public int FacebookUid { get; set; }

        [JsonProperty("appleUid")]
        public object AppleUid { get; set; }

        [JsonProperty("googleUid")]
        public object GoogleUid { get; set; }

        [JsonProperty("newUser")]
        public bool NewUser { get; set; }
    }


    class Program
    {

        public static async Task<JsonTidalAccountRefreshAccess> RefreshToken(string refreshToken)
        {
            string clientID = "zU4XHVVkc2tDPo4t";
            string clientSecret = "VJKhDFqJPqvsPVNBV6ukXTJmwlvbttP7wlMlrc72se4=";

            GenericList<FormKeypair> formData = new GenericList<FormKeypair>();
            formData.Add(new FormKeypair("client_id", clientID));
            formData.Add(new FormKeypair("refresh_token", refreshToken));
            formData.Add(new FormKeypair("grant_type", "refresh_token"));
            formData.Add(new FormKeypair("scope", "r_usr+w_usr+w_sub"));

            RequestData requestData = new RequestData(new Uri("https://auth.tidal.com/v1/oauth2/token"),
                EnumRequestMethod.POST,
                new EnumContentType[] { EnumContentType.FORM },
                new EnumEncodingType[] { EnumEncodingType.UTF8 },
                formData);

            requestData.Header.Add("accept-encoding", "gzip, deflate, br");
            requestData.Accept = "*/*";

            string authToken = Convert.ToBase64String(Encoding.Default.GetBytes(clientID + ":" + clientSecret));
            requestData.AddAuthMethod(new Auth(authToken, EnumAuthType.BASIC));

            JsonTidalAccountRefreshAccess accountAccess = null;

            try
            {
                ResponseData response = await new Request(requestData).GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                if (response.GetContentAsString().Contains("authorization_pending"))
                    return null;

                accountAccess = JsonConvert.DeserializeObject<JsonTidalAccountRefreshAccess>(response.GetContentAsString());

                if (accountAccess == null)
                    return null;
            }
            catch (Exception e)
            { }

            return accountAccess;
        }

        static void Main(string[] args)
        {
            //Task.Factory.StartNew(async () =>
            //{
            //    string refreshToken =
            //        "eyJraWQiOiJoUzFKYTdVMCIsImFsZyI6IkVTNTEyIn0.eyJ0eXBlIjoibzJfcmVmcmVzaCIsInVpZCI6MTg3MzkyNzg2LCJzY29wZSI6Indfc3ViIHJfdXNyIHdfdXNyIiwiY2lkIjozMjM1LCJzVmVyIjowLCJnVmVyIjowLCJpc3MiOiJodHRwczovL2F1dGgudGlkYWwuY29tL3YxIn0.Ac7-qBekj52NS0hwgT0rOY8s_NsF5GOuDFhBqTu0XlV6z35YT6yJONzeCl7DcxF9rq_zVo5Zo7Oljk-rZNzPu0lSAODciz0vEfOrT57lazK0qEf3yQFvkvAXD-qUcEpQ_zWqgrglfON0ArLZo1YEZmNtX4iUwXrKgF6JQ3jR7ENJ8ElJ";

            //    JsonTidalAccountRefreshAccess refresh =
            //        await RefreshToken(refreshToken);

            //    //Console.WriteLine(refresh.AccessToken);
            //});


            //PauseTokenSource pause = new PauseTokenSource();


            //Task.Factory.StartNew(async () =>
            //{
            //    await pause.PauseIfRequestedAsync();

            //});

            //RequestCache.INSTANCE.CachingAllowed = true;

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //RequestData requestData = new RequestData("https://google.com");
            //Request r = new Request(requestData);
            //r.GetResponse().GetContentAsString();

            //sw.Stop();
            //Console.WriteLine("1st try took " + sw.ElapsedMilliseconds);

            //sw.Reset();
            //sw.Start();

            //RequestData requestData2 = new RequestData("https://google.com");
            //Request r2 = new Request(requestData2);
            //var fen = r2.GetResponse().GetContentAsString();

            //sw.Stop();
            //Console.WriteLine("2nd try took " + sw.ElapsedMilliseconds);

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

            //TaskRegister register = new TaskRegister();

            //object obj1 = new object();
            //object obj2 = new object();
            //object obj3 = new object();


            //TaskRegister newTaskRegister = new TaskRegister();

            //TaskSuspensionToken taskSuspensionToken = null;

            //newTaskRegister.RegisterTask(out taskSuspensionToken, async () =>
            //{
            //    while (true)
            //    {
            //        await Task.Delay(100);
            //        await taskSuspensionToken.WaitForRelease();

            //        Console.WriteLine("Fenneg");
            //    }
            //}, obj1);

            //bool suspend = false;

            //Console.WriteLine("Suspend");
            //newTaskRegister.Suspend(obj1);

            //Console.WriteLine("Resume");
            //Thread.Sleep(1000);
            //newTaskRegister.Resume(obj1);

            //Console.WriteLine("Suspend");
            //Thread.Sleep(3000);
            //newTaskRegister.Suspend(obj1);

            //Console.WriteLine("Resume");
            //Thread.Sleep(3000);
            //newTaskRegister.Resume(obj1);


            //CancellationTokenSource cancellationToken = new CancellationTokenSource();

            //register.RegisterTask(async () =>
            //{
            //    while (true)
            //    {
            //        if (register.IsCancelationRequested(obj1))
            //            return;

            //        await Task.Delay(100);
            //        Console.WriteLine("Hello from type1");
            //    }
            //}, obj1, true);

            //register.RegisterTask(async () =>
            //{
            //    while (true)
            //    {
            //        if (register.IsCancelationRequested(obj2))
            //            return;

            //        await Task.Delay(100);

            //        Console.WriteLine("Hello from type2");
            //    }
            //}, obj2, true);

            //register.RegisterTask(async () =>
            //{
            //    while (true)
            //    {
            //        if (register.IsCancelationRequested(obj3))
            //            return;

            //        await Task.Delay(100);

            //        Console.WriteLine("Hello from type3");
            //    }
            //}, obj3, true);

            //register.Suspend(obj2);
            //register.Resume(obj2);

            //register.RegisterTask(new Task(async () =>
            //{
            //    while (true)
            //    {
            //        await Task.Delay(100);
            //        Console.WriteLine("Hello from type2");
            //    }
            //}), "test2", true);

            //register.RegisterTask(new Task(async () =>
            //{
            //    while (true)
            //    {
            //        await Task.Delay(100);
            //        Console.WriteLine("Hello from type3");
            //    }
            //}), "test3", true);

            //register.Suspend("test2");
            //register.Resume("test2");

            Console.ReadKey();

            //List<string> list = new List<string>();
            //list.Add("fenneg1");
            //list.Add("fenneg2");
            //list.Add("fenneg3");
            //list.Add("fenneg4");

            //GenericList<string> genericList = new GenericList<string>(list);
            //var f = genericList.GetRangeAsList(0, 4);

            //genericList.Remove("fenneg3");

            //genericList.GetAsList().ForEach(t => Console.WriteLine(t));

            //Console.ReadKey();

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
