using System.Diagnostics;
using System.Net;
using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;
using DevBase.Net.Core;
using DevBase.Net.Data.Header.UserAgent.Bogus.Generator;
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
                .WithEncodedForm(
                    ("jope", "mama"),
                    ("hello", "kitty")
                )
                .WithJsonBody<Person>(p)
                .WithRetryPolicy(new RetryPolicy()
                {
                    MaxDelay = TimeSpan.FromSeconds(1),
                    MaxRetries = 2
                }).WithLogging(new LoggingConfig()
                {
                    Logger = l
                })
            
                .WithScrapingBypass(new ScrapingBypassConfig()
                {
                    BrowserProfile = EnumBrowserProfile.Firefox
                }).WithHeader("sec-fetch-mode", "yoemamam")
                .WithUrl("https://www.cloudflare.com/rate-limit-test/");
            Response response = await request.SendAsync();

            string data = await response.GetStringAsync();
            data.DumpConsole();
        }
    }
}