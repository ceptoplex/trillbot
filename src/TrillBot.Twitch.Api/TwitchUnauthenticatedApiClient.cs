using System;
using System.Collections.Generic;
using System.Net.Http;
using RestEase;
using TrillBot.Twitch.Api.Api;
using TrillBot.Twitch.Api.Api.Authenticated;

namespace TrillBot.Twitch.Api
{
    internal class TwitchUnauthenticatedApiClient : ITwitchUnauthenticatedApiClient, IDisposable
    {
        private static readonly Uri AuthenticationBaseUri = new Uri("https://id.twitch.tv/oauth2");

        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly ICollection<HttpClient> _httpClients = new List<HttpClient>();

        public TwitchUnauthenticatedApiClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ITwitchAuthenticationApi ITwitchUnauthenticatedApiClient.Authentication()
        {
            var api = CreateApi<ITwitchAuthenticationApi>(AuthenticationBaseUri);
            api.ClientId = _clientId;
            api.ClientSecret = _clientSecret;
            return api;
        }

        protected TApi CreateApi<TApi>(Uri baseUrl, HttpMessageHandler httpMessageHandler = default)
        {
            var httpClient = httpMessageHandler != default
                ? new HttpClient(httpMessageHandler)
                : new HttpClient();
            httpClient.BaseAddress = baseUrl;
            _httpClients.Add(httpClient);

            var restClient = new RestClient(httpClient);

            var api = restClient.For<TApi>();
            if (api is ITwitchAuthenticatedApi authenticatedApi)
                authenticatedApi.ClientId = _clientId;
            return api;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            foreach (var httpClient in _httpClients)
                httpClient.Dispose();
        }
    }
}