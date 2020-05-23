using System;
using System.Net.Http;
using Newtonsoft.Json;
using RestEase;
using RestEase.Implementation;
using TrillBot.Common.Converters;

namespace TrillBot.WebSub.Api
{
    internal sealed class HubApiClient : IHubApiClient, IDisposable
    {
        private readonly IWebSubHubApiRequestAuthenticator _hubApiRequestAuthenticator;
        private readonly Uri _hubUrl;

        private HttpClient _httpClient;

        public HubApiClient(Uri hubUrl, IWebSubHubApiRequestAuthenticator hubApiRequestAuthenticator = default)
        {
            _hubUrl = hubUrl;
            _hubApiRequestAuthenticator = hubApiRequestAuthenticator;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public IHubApi Hub()
        {
            if (_httpClient == null)
                _httpClient = new HttpClient(
                    new ModifyingClientHttpHandler(
                        async (request, cancellationToken) =>
                        {
                            if (_hubApiRequestAuthenticator == default) return;

                            await _hubApiRequestAuthenticator.AuthenticateAsync(
                                request,
                                cancellationToken);
                        }))
                {
                    BaseAddress = _hubUrl
                };

            var restClient = new RestClient(_httpClient)
            {
                JsonSerializerSettings = new JsonSerializerSettings()
            };
            restClient.JsonSerializerSettings.Converters.Add(new SecondsToTimeSpanConverter());
            return restClient.For<IHubApi>();
        }
    }
}