using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Twitch.Api;
using TrillBot.Twitch.Api.Entities.Webhooks;
using TrillBot.WebSub;

namespace TrillBot.Twitch.Webhooks
{
    internal sealed class TwitchWebhooksWebSubSubscriptionMetadataResolver : IWebSubSubscriptionMetadataResolver
    {
        private readonly ITwitchApiClient _apiClient;
        private IEnumerable<TwitchWebhookSubscriptionDto> _webhookSubscriptions;

        public TwitchWebhooksWebSubSubscriptionMetadataResolver(ITwitchApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task UpdateSubscriptionMetadataAsync(
            IWebSubSubscriptionMetadata subscriptionMetadata,
            CancellationToken cancellationToken = default)
        {
            await EnsureWebhooksCachedAsync(cancellationToken);

            // The fetched subscription will probably be unsubscribed and replaced by the subscription service.
            // Other old subscriptions that are not picked up here are only removed using the expiration mechanism.
            // To make this happen sooner rather than later,
            // always try to catch the subscription with the longest time until expiration here.
            var webhookSubscriptions = _webhookSubscriptions.OrderByDescending(_ => _.ExpiresAt);
            foreach (var webhookSubscription in webhookSubscriptions)
            {
                if (webhookSubscription.Topic != subscriptionMetadata.Topic)
                    continue;

                var match = Regex.Match(
                    webhookSubscription.Callback.ToString(),
                    "^https?://.*" +
                    "(?<contentTypeId>[0-9a-f]{8}-([0-9a-f]{4}-){3}[0-9a-f]{12})/" +
                    "(?<id>[0-9a-f]{8}-([0-9a-f]{4}-){3}[0-9a-f]{12})$");
                if (!match.Success)
                    continue;

                var contentTypeId = Guid.Parse(match.Groups["contentTypeId"].Value);
                if (contentTypeId != subscriptionMetadata.ContentType.GUID)
                    continue;

                var id = Guid.Parse(match.Groups["id"].Value);
                subscriptionMetadata.HandleSubscriptionRequest(id, webhookSubscription.ExpiresAt);
                subscriptionMetadata.HandleVerification(true);
                return;
            }
        }

        private async Task EnsureWebhooksCachedAsync(CancellationToken cancellationToken = default)
        {
            if (_webhookSubscriptions != null)
                return;

            _webhookSubscriptions =
                (await _apiClient.Webhooks().GetSubscriptionsAsync(cancellationToken)).Data
                .OrderByDescending(_ => _.ExpiresAt);
        }
    }
}