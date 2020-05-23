using TrillBot.Common.Events;
using TrillBot.Twitch.Webhooks.EventArgs;

namespace TrillBot.Twitch.Webhooks
{
    public interface ITwitchWebhooks
    {
        event AsyncEventHandler<TwitchStreamOnlineEventArgs> StreamOnline;
        event AsyncEventHandler<TwitchStreamOfflineEventArgs> StreamOffline;
    }
}