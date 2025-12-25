using System.Diagnostics;
using System.Net;
using DevBase.IO;
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;
using DevBase.Net.Core;
using DevBase.Net.Data.Header.UserAgent.Bogus.Generator;
using DevBase.Net.Proxy;
using DevBase.Net.Proxy.Enums;
using DevBase.Net.Security.Token;
using DevBase.Net.Validation;
using Dumpify;
using Newtonsoft.Json.Linq;
using Serilog;

namespace DevBaseLive;

record Person(string name, int age);

class Program
{
    public static async Task Main(string[] args)
    {
        Person p = new Person("alex", 1);

        var l = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.Information()
            .CreateLogger();
        
        for (int i = 0; i < 20; i++)
        {
            Request request = new Request()
                .AsGet()
                .WithHostCheck(new HostCheckConfig())
                .WithProxy(new ProxyInfo("isp.oxylabs.io", 8004, "user-isp_user_NXTYV", "rtVVhrth4545++A", EnumProxyType.Socks5h))
                .UseBasicAuthentication("joe", "mama")
                .WithRetryPolicy(new RetryPolicy()
                {
                    MaxRetries = 2
                })
                .WithLogging(new LoggingConfig()
                {
                    Logger = l
                })
                .WithMultipleFiles(
                    ("file1", AFile.ReadFileToObject("C:\\Users\\alex\\Desktop\\zoom1.txt")),
                    ("file2", AFile.ReadFileToObject("C:\\Users\\alex\\Desktop\\zoom2.txt"))
                    )
            
                .WithScrapingBypass(new ScrapingBypassConfig()
                {
                    BrowserProfile = EnumBrowserProfile.Firefox
                }).WithHeader("sec-fetch-mode", "yoemamam")
                .WithUrl("https://webhook.site/bd100268-d633-43f5-b298-28ee17c97ccf");
            Response response = await request.SendAsync();

            string data = await response.GetStringAsync();
            data.DumpConsole();
        }
    }
}