using DevBase.Requests.Core;

namespace DevBase.Requests.Interfaces;

public interface IRequestInterceptor
{
    Task OnRequestAsync(Request request, CancellationToken cancellationToken = default);
    int Order => 0;
}
