using System.Threading;
using System.Threading.Tasks;
using RestEase;
using TrillBot.Twitch.Api.Entities;
using TrillBot.Twitch.Api.Entities.Webhooks;

namespace TrillBot.Twitch.Api.Api.Authenticated
{
    public interface ITwitchWebhooksApi : ITwitchAppAuthenticatedApi
    {
        [Get("subscriptions")]
        Task<TwitchResponseWrapperDto<TwitchWebhookSubscriptionDto>> GetSubscriptionsAsync(
            CancellationToken cancellationToken = default);
    }
}