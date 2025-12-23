namespace DevBase.Net.Interfaces;

public interface IRequestInterceptor
{
    Task OnRequestAsync(Core.Request request, CancellationToken cancellationToken = default);
    int Order => 0;
}
