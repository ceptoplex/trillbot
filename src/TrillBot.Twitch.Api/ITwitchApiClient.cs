using TrillBot.Twitch.Api.Api.Authenticated;

namespace TrillBot.Twitch.Api
{
    public interface ITwitchApiClient : ITwitchUnauthenticatedApiClient
    {
        ITwitchWebhooksApi Webhooks();
    }
}