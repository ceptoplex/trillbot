using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Twitch.Api.Api.Authenticated;
using TrillBot.Twitch.Api.Authentication;
using TrillBot.WebSub.Api;

namespace TrillBot.Twitch.Webhooks.Api
{
    internal sealed class TwitchWebSubHubApiRequestAuthenticator : IWebSubHubApiRequestAuthenticator
    {
        private readonly ITwitchAppAccessTokenProvider _appAccessTokenProvider;
        private readonly string _clientId;

        public TwitchWebSubHubApiRequestAuthenticator(
            ITwitchAppAccessTokenProvider appAccessTokenProvider,
            string clientId)
        {
            _appAccessTokenProvider = appAccessTokenProvider;
            _clientId = clientId;
        }

        public async Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var accessToken = await _appAccessTokenProvider.GetAccessTokenAsync(cancellationToken);
            request.Headers.Add("Client-Id", _clientId);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                ITwitchAppAuthenticatedApi.AuthenticationScheme,
                accessToken);
        }
    }
}