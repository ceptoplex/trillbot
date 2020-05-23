using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using RestEase.Implementation;
using TrillBot.Twitch.Api.Api.Authenticated;
using TrillBot.Twitch.Api.Authentication;

namespace TrillBot.Twitch.Api
{
    internal sealed class TwitchApiClient : TwitchUnauthenticatedApiClient, ITwitchApiClient
    {
        private static readonly Uri WebhooksBaseUri = new Uri("https://api.twitch.tv/helix/webhooks");
        private readonly ITwitchAppAccessTokenProvider _appAccessTokenProvider;

        private readonly string _clientId;

        public TwitchApiClient(
            ITwitchAppAccessTokenProvider appAccessTokenProvider,
            string clientId,
            string clientSecret) : base(
            clientId,
            clientSecret)
        {
            _appAccessTokenProvider = appAccessTokenProvider;
            _clientId = clientId;
        }

        public ITwitchWebhooksApi Webhooks()
        {
            return CreateAppAuthenticatedApi<ITwitchWebhooksApi>(WebhooksBaseUri);
        }

        private TAppAuthenticatedApi CreateAppAuthenticatedApi<TAppAuthenticatedApi>(Uri baseUrl)
            where TAppAuthenticatedApi : ITwitchAppAuthenticatedApi
        {
            var api = CreateApi<TAppAuthenticatedApi>(
                baseUrl,
                new ModifyingClientHttpHandler(
                    async (request, cancellationToken) =>
                    {
                        await EnsureAppAuthenticationAsync(
                            request,
                            _appAccessTokenProvider,
                            cancellationToken);
                    }));
            api.ClientId = _clientId;
            return api;
        }

        private static async Task EnsureAppAuthenticationAsync(
            HttpRequestMessage request,
            ITwitchAppAccessTokenProvider appAccessTokenProvider,
            CancellationToken cancellationToken = default)
        {
            var accessToken = await appAccessTokenProvider.GetAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                ITwitchAppAuthenticatedApi.AuthenticationScheme,
                accessToken);
        }
    }
}