using System.Diagnostics;
using System.Net;
using DevBase.Requests.Configuration;
using DevBase.Requests.Core;
using DevBase.Requests.Security.Token;
using DevBase.Requests.Validation;

namespace DevBaseLive;

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
    private static int _passedTests;
    private static int _failedTests;
    private static readonly List<TestResult> _testResults = new();

    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        PrintHeader("DevBase.Requests Test Suite");
        Console.WriteLine($"  Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine();

        // Warmup
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  Warming up HTTP connections...");
        Console.ResetColor();
        await WarmupAsync();
        Console.WriteLine();

        // === FUNCTIONAL TESTS ===
        PrintHeader("Functional Tests");
        
        await RunTestAsync("GET Request - Basic", TestBasicGetRequest);
        await RunTestAsync("GET Request - JSON Parsing", TestJsonParsing);
        await RunTestAsync("GET Request - JsonPath Extraction", TestJsonPathExtraction);
        await RunTestAsync("POST Request - Form Data", TestPostFormData);
        await RunTestAsync("Header Validation - Valid Accept", TestHeaderValidationAccept);
        await RunTestAsync("Header Validation - Valid Content-Type", TestHeaderValidationContentType);
        await RunTestAsync("JWT Token - Parsing", TestJwtTokenParsing);
        await RunTestAsync("JWT Token - Expiration Check", TestJwtTokenExpiration);
        await RunTestAsync("Request - Custom Headers", TestCustomHeaders);
        await RunTestAsync("Request - Timeout Handling", TestTimeoutHandling);
        
        Console.WriteLine();

        // === PERFORMANCE TESTS ===
        PrintHeader("Performance Tests");
        
        var perfResults = await RunPerformanceTestsAsync();
        
        Console.WriteLine();

        // === COMPARISON TESTS ===
        PrintHeader("Comparison: DevBase vs HttpClient");
        
        await RunComparisonTestAsync("Response Time", perfResults);
        await RunComparisonTestAsync("Data Integrity", perfResults);
        
        Console.WriteLine();

        // === TEST SUMMARY ===
        PrintTestSummary();
        
        Console.WriteLine();
        Console.WriteLine("  Press any key to exit...");
        Console.ReadKey();
    }

    #region Test Runner

    private static async Task RunTestAsync(string testName, Func<Task<(bool passed, string message, object? expected, object? actual)>> testFunc)
    {
        Console.Write($"  {testName,-45}");
        
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            var (passed, message, expected, actual) = await testFunc();
            sw.Stop();

            var result = new TestResult(testName, passed, message, sw.ElapsedMilliseconds, expected, actual);
            _testResults.Add(result);

            if (passed)
            {
                _passedTests++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"PASS  ({sw.ElapsedMilliseconds}ms)");
            }
            else
            {
                _failedTests++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FAIL  ({sw.ElapsedMilliseconds}ms)");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"       Expected: {expected}");
                Console.WriteLine($"       Actual:   {actual}");
                Console.WriteLine($"       Message:  {message}");
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            _failedTests++;
            
            var result = new TestResult(testName, false, ex.Message, sw.ElapsedMilliseconds, "No exception", ex.GetType().Name);
            _testResults.Add(result);
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR ({sw.ElapsedMilliseconds}ms)");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"       Exception: {ex.GetType().Name}: {ex.Message}");
        }
        
        Console.ResetColor();
    }

    #endregion

    #region Functional Tests

    private static async Task<(bool, string, object?, object?)> TestBasicGetRequest()
    {
        Response response = await new Request("https://httpbin.org/get")
            .AsGet()
            .SendAsync();
        
        bool passed = response.StatusCode == HttpStatusCode.OK;
        return (passed, "Basic GET request", HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task<(bool, string, object?, object?)> TestJsonParsing()
    {
        Response response = await new Request("https://api.n.exchange/en/api/v1/currency/")
            .AsGet()
            .WithAcceptJson()
            .SendAsync();

        List<Currency>? currencies = await response.ParseJsonAsync<List<Currency>>();
        
        bool passed = currencies != null && currencies.Count > 0;
        return (passed, "JSON parsing returns data", "> 0 items", currencies?.Count.ToString() ?? "null");
    }

    private static async Task<(bool, string, object?, object?)> TestJsonPathExtraction()
    {
        Response response = await new Request("https://api.n.exchange/en/api/v1/currency/")
            .AsGet()
            .WithAcceptJson()
            .SendAsync();

        List<string> names = await response.ParseJsonPathListAsync<string>("$[*].name");
        
        bool passed = names.Count > 0 && names.All(n => !string.IsNullOrEmpty(n));
        return (passed, "JsonPath extracts names", "> 0 names", names.Count.ToString());
    }

    private static async Task<(bool, string, object?, object?)> TestPostFormData()
    {
        Response response = await new Request("https://httpbin.org/post")
            .AsPost()
            .WithEncodedForm(("key1", "value1"), ("key2", "value2"))
            .SendAsync();
        
        bool passed = response.StatusCode == HttpStatusCode.OK;
        string content = await response.GetStringAsync();
        bool hasFormData = content.Contains("key1") && content.Contains("value1");
        
        return (passed && hasFormData, "POST with form data", "Contains form data", hasFormData.ToString());
    }

    private static async Task<(bool, string, object?, object?)> TestHeaderValidationAccept()
    {
        ValidationResult result = HeaderValidator.ValidateAccept("application/json");
        return (result.IsValid, "Accept header validation", true, result.IsValid);
    }

    private static async Task<(bool, string, object?, object?)> TestHeaderValidationContentType()
    {
        ValidationResult result = HeaderValidator.ValidateContentType("application/json; charset=utf-8");
        return (result.IsValid, "Content-Type validation", true, result.IsValid);
    }

    private static async Task<(bool, string, object?, object?)> TestJwtTokenParsing()
    {
        // Sample JWT token (expired, but valid format)
        string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        
        AuthenticationToken? token = HeaderValidator.ParseJwtToken(jwt);
        
        bool passed = token != null && token.Payload.Subject == "1234567890";
        return (passed, "JWT parsing extracts claims", "1234567890", token?.Payload.Subject ?? "null");
    }

    private static async Task<(bool, string, object?, object?)> TestJwtTokenExpiration()
    {
        // Expired JWT token
        string expiredJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNTE2MjM5MDIyfQ.4Adcj3UFYzPUVaVF43FmMab6RlaQD8A9V8wFzzht-KQ";
        
        ValidationResult result = HeaderValidator.ValidateJwtToken(expiredJwt, checkExpiration: true);
        
        bool passed = !result.IsValid && result.ErrorMessage!.Contains("expired");
        return (passed, "Detects expired JWT", "expired", result.IsValid ? "valid" : "expired");
    }

    private static async Task<(bool, string, object?, object?)> TestCustomHeaders()
    {
        Response response = await new Request("https://httpbin.org/headers")
            .AsGet()
            .WithHeader("X-Custom-Header", "TestValue123")
            .SendAsync();
        
        string content = await response.GetStringAsync();
        bool hasHeader = content.Contains("X-Custom-Header") && content.Contains("TestValue123");
        
        return (hasHeader, "Custom headers sent", true, hasHeader);
    }

    private static async Task<(bool, string, object?, object?)> TestTimeoutHandling()
    {
        try
        {
            await new Request("https://httpbin.org/delay/10")
                .AsGet()
                .WithTimeout(TimeSpan.FromMilliseconds(500))
                .SendAsync();
            
            return (false, "Should have timed out", "Timeout exception", "No exception");
        }
        catch (TaskCanceledException)
        {
            return (true, "Timeout triggered correctly", "Timeout", "Timeout");
        }
        catch (OperationCanceledException)
        {
            return (true, "Timeout triggered correctly", "Timeout", "Timeout");
        }
        catch (DevBase.Requests.Exception.RequestTimeoutException)
        {
            return (true, "Timeout triggered correctly", "Timeout", "Timeout");
        }
    }

    #endregion

    #region Performance Tests

    private static async Task<PerformanceResults> RunPerformanceTestsAsync()
    {
        const int iterations = 5;
        
        Console.WriteLine($"  Running {iterations} iterations for each method...");
        Console.WriteLine();

        // DevBase.Requests + JsonPath
        Console.Write($"  {"DevBase + JsonPath",-35}");
        List<long> jsonPathTimes = new();
        List<string> jsonPathData = new();
        
        for (int i = 0; i < iterations; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            jsonPathData = await TestDevBaseJsonPathAsync();
            sw.Stop();
            jsonPathTimes.Add(sw.ElapsedMilliseconds);
        }
        
        double jsonPathAvg = jsonPathTimes.Skip(1).Average();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Avg: {jsonPathAvg:F1}ms (Min: {jsonPathTimes.Skip(1).Min()}ms, Max: {jsonPathTimes.Skip(1).Max()}ms)");
        Console.ResetColor();

        // HttpClient + Full Deserialization
        Console.Write($"  {"HttpClient + Full Deser.",-35}");
        List<long> httpClientTimes = new();
        List<string> httpClientData = new();
        
        for (int i = 0; i < iterations; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            httpClientData = await TestManualHttpClientAsync();
            sw.Stop();
            httpClientTimes.Add(sw.ElapsedMilliseconds);
        }
        
        double httpClientAvg = httpClientTimes.Skip(1).Average();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Avg: {httpClientAvg:F1}ms (Min: {httpClientTimes.Skip(1).Min()}ms, Max: {httpClientTimes.Skip(1).Max()}ms)");
        Console.ResetColor();

        // DevBase.Requests + Full Deserialization
        Console.Write($"  {"DevBase + Full Deser.",-35}");
        List<long> devbaseFullTimes = new();
        List<string> devbaseFullData = new();
        
        for (int i = 0; i < iterations; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            devbaseFullData = await TestDevBaseFullDeserializationAsync();
            sw.Stop();
            devbaseFullTimes.Add(sw.ElapsedMilliseconds);
        }
        
        double devbaseFullAvg = devbaseFullTimes.Skip(1).Average();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Avg: {devbaseFullAvg:F1}ms (Min: {devbaseFullTimes.Skip(1).Min()}ms, Max: {devbaseFullTimes.Skip(1).Max()}ms)");
        Console.ResetColor();

        return new PerformanceResults(
            jsonPathAvg, httpClientAvg, devbaseFullAvg,
            jsonPathData, httpClientData, devbaseFullData);
    }

    private static async Task RunComparisonTestAsync(string testName, PerformanceResults results)
    {
        Console.Write($"  {testName,-45}");
        
        bool passed;
        string expected, actual;

        if (testName == "Response Time")
        {
            double diff = ((results.HttpClientAvg - results.DevBaseFullAvg) / results.HttpClientAvg) * 100;
            passed = results.DevBaseFullAvg <= results.HttpClientAvg * 1.5; // Allow 50% variance
            expected = $"DevBase <= HttpClient * 1.5";
            actual = $"DevBase: {results.DevBaseFullAvg:F1}ms vs HttpClient: {results.HttpClientAvg:F1}ms ({(diff > 0 ? "+" : "")}{diff:F1}%)";
        }
        else // Data Integrity
        {
            bool sameCount = results.JsonPathData.Count == results.HttpClientData.Count 
                          && results.HttpClientData.Count == results.DevBaseFullData.Count;
            bool sameContent = results.JsonPathData.SequenceEqual(results.HttpClientData) 
                            && results.HttpClientData.SequenceEqual(results.DevBaseFullData);
            
            passed = sameCount && sameContent;
            expected = $"All methods return same data";
            actual = $"Count: {results.JsonPathData.Count}/{results.HttpClientData.Count}/{results.DevBaseFullData.Count}, Match: {sameContent}";
        }

        var result = new TestResult(testName, passed, "", 0, expected, actual);
        _testResults.Add(result);

        if (passed)
        {
            _passedTests++;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("PASS");
        }
        else
        {
            _failedTests++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FAIL");
        }
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"       {actual}");
        Console.ResetColor();
    }

    #endregion

    #region Helper Methods

    private static async Task WarmupAsync()
    {
        using HttpClient client = new HttpClient();
        await client.GetStringAsync("https://httpbin.org/get");
        
        Response response = await new Request("https://httpbin.org/get").SendAsync();
        await response.GetStringAsync();
    }

    private static async Task<List<string>> TestDevBaseJsonPathAsync()
    {
        Response response = await new Request("https://api.n.exchange/en/api/v1/currency/")
            .AsGet()
            .WithAcceptJson()
            .SendAsync();

        return await response.ParseJsonPathListAsync<string>("$[*].name");
    }

    private static async Task<List<string>> TestManualHttpClientAsync()
    {
        using HttpClient client = new HttpClient();
        string json = await client.GetStringAsync("https://api.n.exchange/en/api/v1/currency/");
        List<Currency>? currencies = System.Text.Json.JsonSerializer.Deserialize<List<Currency>>(json);
        return currencies?.Select(c => c.name).ToList() ?? new List<string>();
    }

    private static async Task<List<string>> TestDevBaseFullDeserializationAsync()
    {
        Response response = await new Request("https://api.n.exchange/en/api/v1/currency/")
            .AsGet()
            .WithAcceptJson()
            .SendAsync();

        List<Currency>? currencies = await response.ParseJsonAsync<List<Currency>>();
        return currencies?.Select(c => c.name).ToList() ?? new List<string>();
    }

    private static void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"╔{'═'.ToString().PadRight(58, '═')}╗");
        Console.WriteLine($"║  {title.PadRight(56)}║");
        Console.WriteLine($"╚{'═'.ToString().PadRight(58, '═')}╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void PrintTestSummary()
    {
        Console.WriteLine();
        PrintHeader("Test Results Summary");
        
        int total = _passedTests + _failedTests;
        double passRate = total > 0 ? (_passedTests * 100.0 / total) : 0;
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  Total Tests:    {total}");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  Passed:         {_passedTests}");
        
        Console.ForegroundColor = _failedTests > 0 ? ConsoleColor.Red : ConsoleColor.Green;
        Console.WriteLine($"  Failed:         {_failedTests}");
        
        Console.ForegroundColor = passRate >= 80 ? ConsoleColor.Green : (passRate >= 50 ? ConsoleColor.Yellow : ConsoleColor.Red);
        Console.WriteLine($"  Pass Rate:      {passRate:F1}%");
        Console.ResetColor();
        Console.WriteLine();

        if (_failedTests > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Failed Tests:");
            Console.ResetColor();
            
            foreach (var test in _testResults.Where(t => !t.Passed))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"    ✗ {test.Name}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"      Expected: {test.Expected}");
                Console.WriteLine($"      Actual:   {test.Actual}");
                Console.ResetColor();
            }
        }
        
        Console.WriteLine();
        Console.ForegroundColor = passRate == 100 ? ConsoleColor.Green : ConsoleColor.Yellow;
        Console.WriteLine(passRate == 100 
            ? "  ✓ All tests passed!" 
            : $"  ⚠ {_failedTests} test(s) need attention");
        Console.ResetColor();
    }

    #endregion
}

#region Records

record TestResult(string Name, bool Passed, string Message, long DurationMs, object? Expected, object? Actual);
record PerformanceResults(
    double JsonPathAvg, 
    double HttpClientAvg, 
    double DevBaseFullAvg,
    List<string> JsonPathData,
    List<string> HttpClientData,
    List<string> DevBaseFullData);

#endregion