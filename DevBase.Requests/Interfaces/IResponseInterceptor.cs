namespace DevBase.Requests.Interfaces;

public interface IResponseInterceptor
{
    Task OnResponseAsync(Response response, CancellationToken cancellationToken = default);
    int Order => 0;
}
