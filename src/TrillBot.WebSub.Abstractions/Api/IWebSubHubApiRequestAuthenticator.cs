using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.WebSub.Api
{
    public interface IWebSubHubApiRequestAuthenticator
    {
        Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }
}