using System.Diagnostics;
using System.Globalization;
using System.Text;
using CsvHelper;
using DevBase.Requests.Configuration;
using DevBase.Requests.Core;
using DevBaseLive.Objects;
using DevBaseLive.Tracks;
using Newtonsoft.Json;

namespace DevBaseLive
{
    public class Currency
    {
        public string code { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int? min_confirmations { get; set; }
        public bool is_crypto { get; set; }
        public string minimal_amount { get; set; } = string.Empty;
        public string maximal_amount { get; set; } = string.Empty;
        public string? contract_address { get; set; }
        public bool is_base_of_enabled_pair { get; set; }
        public bool is_quote_of_enabled_pair { get; set; }
        public bool has_enabled_pairs { get; set; }
        public bool is_base_of_enabled_pair_for_test { get; set; }
        public bool is_quote_of_enabled_pair_for_test { get; set; }
        public bool has_enabled_pairs_for_test { get; set; }
        public string withdrawal_fee { get; set; } = string.Empty;
        public string? extra_id { get; set; }
        public object? network { get; set; }
        public int decimals { get; set; }
    }

    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== DevBase.Requests Performance Test ===");
            Console.WriteLine($"API: https://api.n.exchange/en/api/v1/currency/");
            Console.WriteLine($"Iterations: 10");
            Console.WriteLine();

            // Warmup
            Console.WriteLine("Warming up...");
            await WarmupAsync();
            Console.WriteLine();

            // Test 1: DevBase.Requests with JsonPath (streaming/skipping)
            Console.WriteLine("--- Test 1: DevBase.Requests + JsonPath ---");
            List<long> jsonPathTimes = new List<long>();
            List<string> jsonPathNames = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                jsonPathNames = await TestDevBaseJsonPathAsync();
                sw.Stop();
                jsonPathTimes.Add(sw.ElapsedMilliseconds);
                Console.WriteLine($"  Iteration {i + 1}: {sw.ElapsedMilliseconds}ms ({jsonPathNames.Count} currencies)");
            }

            Console.WriteLine();

            // Test 2: Manual HttpClient + Full Object Deserialization
            Console.WriteLine("--- Test 2: HttpClient + Full Deserialization ---");
            List<long> manualTimes = new List<long>();
            List<string> manualNames = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                manualNames = await TestManualHttpClientAsync();
                sw.Stop();
                manualTimes.Add(sw.ElapsedMilliseconds);
                Console.WriteLine($"  Iteration {i + 1}: {sw.ElapsedMilliseconds}ms ({manualNames.Count} currencies)");
            }

            Console.WriteLine();

            // Test 3: DevBase.Requests + Full Object Deserialization (for fair comparison)
            Console.WriteLine("--- Test 3: DevBase.Requests + Full Deserialization ---");
            List<long> devbaseFullTimes = new List<long>();
            List<string> devbaseFullNames = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                devbaseFullNames = await TestDevBaseFullDeserializationAsync();
                sw.Stop();
                devbaseFullTimes.Add(sw.ElapsedMilliseconds);
                Console.WriteLine($"  Iteration {i + 1}: {sw.ElapsedMilliseconds}ms ({devbaseFullNames.Count} currencies)");
            }

            Console.WriteLine();

            // Results Summary
            Console.WriteLine("=== RESULTS SUMMARY ===");
            Console.WriteLine();
            
            double jsonPathAvg = jsonPathTimes.Skip(1).Average(); // Skip first (cold start)
            double manualAvg = manualTimes.Skip(1).Average();
            double devbaseFullAvg = devbaseFullTimes.Skip(1).Average();

            Console.WriteLine($"DevBase.Requests + JsonPath:");
            Console.WriteLine($"  Average: {jsonPathAvg:F2}ms");
            Console.WriteLine($"  Min: {jsonPathTimes.Skip(1).Min()}ms");
            Console.WriteLine($"  Max: {jsonPathTimes.Skip(1).Max()}ms");
            Console.WriteLine();

            Console.WriteLine($"HttpClient + Full Deserialization:");
            Console.WriteLine($"  Average: {manualAvg:F2}ms");
            Console.WriteLine($"  Min: {manualTimes.Skip(1).Min()}ms");
            Console.WriteLine($"  Max: {manualTimes.Skip(1).Max()}ms");
            Console.WriteLine();

            Console.WriteLine($"DevBase.Requests + Full Deserialization:");
            Console.WriteLine($"  Average: {devbaseFullAvg:F2}ms");
            Console.WriteLine($"  Min: {devbaseFullTimes.Skip(1).Min()}ms");
            Console.WriteLine($"  Max: {devbaseFullTimes.Skip(1).Max()}ms");
            Console.WriteLine();

            Console.WriteLine($"=== COMPARISON ===");
            double diff1 = ((manualAvg - jsonPathAvg) / manualAvg) * 100;
            double diff2 = ((manualAvg - devbaseFullAvg) / manualAvg) * 100;
            
            Console.WriteLine($"JsonPath vs HttpClient: {(diff1 > 0 ? "+" : "")}{diff1:F1}% {(diff1 > 0 ? "faster" : "slower")}");
            Console.WriteLine($"DevBase Full vs HttpClient: {(diff2 > 0 ? "+" : "")}{diff2:F1}% {(diff2 > 0 ? "faster" : "slower")}");
            Console.WriteLine();

            // Show first 10 currency names
            Console.WriteLine("=== Sample Currency Names (first 10) ===");
            foreach (string name in jsonPathNames.Take(10))
            {
                Console.WriteLine($"  - {name}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task WarmupAsync()
        {
            // Warmup both approaches
            using HttpClient client = new HttpClient();
            await client.GetStringAsync("https://api.n.exchange/en/api/v1/currency/");
            
            Response response = await new Request("https://api.n.exchange/en/api/v1/currency/").SendAsync();
            await response.GetStringAsync();
        }

        /// <summary>
        /// Test using DevBase.Requests with JsonPath to extract only the names
        /// </summary>
        private static async Task<List<string>> TestDevBaseJsonPathAsync()
        {
            Response response = await new Request("https://api.n.exchange/en/api/v1/currency/")
                .AsGet()
                .WithAcceptJson()
                .SendAsync();

            // Use JsonPath to extract only names - skips parsing unnecessary fields
            List<string> names = await response.ParseJsonPathListAsync<string>("$[*].name");
            return names;
        }

        /// <summary>
        /// Test using standard HttpClient with full object deserialization
        /// </summary>
        private static async Task<List<string>> TestManualHttpClientAsync()
        {
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync("https://api.n.exchange/en/api/v1/currency/");

            // Full deserialization of all objects
            List<Currency>? currencies = System.Text.Json.JsonSerializer.Deserialize<List<Currency>>(json);
            
            // Extract names
            List<string> names = currencies?.Select(c => c.name).ToList() ?? new List<string>();
            return names;
        }

        /// <summary>
        /// Test using DevBase.Requests with full object deserialization
        /// </summary>
        private static async Task<List<string>> TestDevBaseFullDeserializationAsync()
        {
            Response response = await new Request("https://api.n.exchange/en/api/v1/currency/")
                .AsGet()
                .WithAcceptJson()
                .SendAsync();

            // Full deserialization
            List<Currency>? currencies = await response.ParseJsonAsync<List<Currency>>();
            
            // Extract names
            List<string> names = currencies?.Select(c => c.name).ToList() ?? new List<string>();
            return names;
        }
    }
}