using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.Twitch.Api.Authentication
{
    internal sealed class TwitchCachedAppAccessTokenProvider : ITwitchAppAccessTokenProvider
    {
        private static readonly TimeSpan MinimumRemainingAccessTokenLifetime = TimeSpan.FromMinutes(5);

        private readonly ITwitchUnauthenticatedApiClient _unauthenticatedApiClient;
        private string _accessToken;
        private DateTime? _accessTokenExpires;

        public TwitchCachedAppAccessTokenProvider(ITwitchUnauthenticatedApiClient unauthenticatedApiClient)
        {
            _unauthenticatedApiClient = unauthenticatedApiClient;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            await EnsureAccessTokenAvailableAsync(cancellationToken);
            return _accessToken;
        }

        private async Task EnsureAccessTokenAvailableAsync(CancellationToken cancellationToken = default)
        {
            if (_accessTokenExpires.HasValue &&
                _accessTokenExpires.Value > DateTime.UtcNow + MinimumRemainingAccessTokenLifetime)
                return;

            var response = await _unauthenticatedApiClient.Authentication().GetAppAccessTokenAsync(cancellationToken);
            _accessToken = response.AccessToken;
            _accessTokenExpires = DateTime.UtcNow + response.ExpiresIn;
        }
    }
}