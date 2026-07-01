using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevBase.IntegrationTests.Infrastructure;

internal sealed class LocalHttpTestServer : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Task _serverTask;
    private readonly TaskCompletionSource<CapturedHttpRequest> _request = new();
    private readonly Func<CapturedHttpRequest, LocalHttpResponse> _responseFactory;

    public LocalHttpTestServer(Func<CapturedHttpRequest, LocalHttpResponse> responseFactory)
    {
        this._responseFactory = responseFactory;
        this._listener = new TcpListener(IPAddress.Loopback, 0);
        this._listener.Start();

        IPEndPoint endpoint = (IPEndPoint)this._listener.LocalEndpoint;
        this.Url = new Uri($"http://127.0.0.1:{endpoint.Port}/");
        this._serverTask = this.AcceptOneRequestAsync(this._shutdown.Token);
    }

    public Uri Url { get; }

    public Task<CapturedHttpRequest> Request => this._request.Task;

    public async ValueTask DisposeAsync()
    {
        this._shutdown.Cancel();
        this._listener.Stop();

        try
        {
            await this._serverTask.WaitAsync(TimeSpan.FromSeconds(1));
        }
        catch
        {
        }

        this._shutdown.Dispose();
    }

    private async Task AcceptOneRequestAsync(CancellationToken cancellationToken)
    {
        try
        {
            using TcpClient client = await this._listener.AcceptTcpClientAsync(cancellationToken);
            await using NetworkStream stream = client.GetStream();
            CapturedHttpRequest request = await ReadRequestAsync(stream, cancellationToken);
            this._request.TrySetResult(request);

            LocalHttpResponse response = this._responseFactory(request);
            await WriteResponseAsync(stream, response, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (System.Exception exception)
        {
            this._request.TrySetException(exception);
        }
    }

    private static async Task<CapturedHttpRequest> ReadRequestAsync(
        NetworkStream stream,
        CancellationToken cancellationToken)
    {
        byte[] headerBytes = await ReadUntilHeaderEndAsync(stream, cancellationToken);
        string headerText = Encoding.ASCII.GetString(headerBytes);
        string[] lines = headerText.Split("\r\n", StringSplitOptions.None);
        string[] requestLine = lines[0].Split(' ', 3);
        Dictionary<string, string> headers = ParseHeaders(lines);
        int contentLength = headers.TryGetValue("Content-Length", out string? contentLengthValue)
            ? int.Parse(contentLengthValue)
            : 0;
        byte[] bodyBytes = await ReadExactAsync(stream, contentLength, cancellationToken);

        return new CapturedHttpRequest(
            requestLine[0],
            requestLine[1],
            requestLine[2],
            headers,
            Encoding.UTF8.GetString(bodyBytes));
    }

    private static Dictionary<string, string> ParseHeaders(string[] lines)
    {
        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);

        foreach (string line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) {
                break;
            }

            string[] parts = line.Split(':', 2);
            if (parts.Length == 2) {
                headers[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return headers;
    }

    private static async Task<byte[]> ReadUntilHeaderEndAsync(
        NetworkStream stream,
        CancellationToken cancellationToken)
    {
        List<byte> bytes = [];
        byte[] current = new byte[1];

        while (!EndsWithHeaderEnd(bytes))
        {
            int read = await stream.ReadAsync(current.AsMemory(0, 1), cancellationToken);
            if (read == 0) {
                break;
            }

            bytes.Add(current[0]);
        }

        return bytes.ToArray();
    }

    private static bool EndsWithHeaderEnd(List<byte> bytes)
    {
        return bytes.Count >= 4 &&
               bytes[^4] == '\r' &&
               bytes[^3] == '\n' &&
               bytes[^2] == '\r' &&
               bytes[^1] == '\n';
    }

    private static async Task<byte[]> ReadExactAsync(
        NetworkStream stream,
        int length,
        CancellationToken cancellationToken)
    {
        byte[] bytes = new byte[length];
        int offset = 0;

        while (offset < length)
        {
            int read = await stream.ReadAsync(bytes.AsMemory(offset, length - offset), cancellationToken);
            if (read == 0) {
                break;
            }

            offset += read;
        }

        return bytes;
    }

    private static async Task WriteResponseAsync(
        NetworkStream stream,
        LocalHttpResponse response,
        CancellationToken cancellationToken)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(response.Body);
        string headers =
            $"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}\r\n" +
            $"Content-Type: {response.ContentType}\r\n" +
            $"Content-Length: {bodyBytes.Length}\r\n" +
            "Connection: close\r\n" +
            "\r\n";
        byte[] headerBytes = Encoding.ASCII.GetBytes(headers);

        await stream.WriteAsync(headerBytes, cancellationToken);
        await stream.WriteAsync(bodyBytes, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }
}
