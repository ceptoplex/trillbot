using System.Linq;
using TrillBot.Common.Events;
using TrillBot.Common.Extensions;
using TrillBot.Twitch.Webhooks.Entities;
using TrillBot.Twitch.Webhooks.EventArgs;
using TrillBot.WebSub;

namespace TrillBot.Twitch.Webhooks
{
    internal sealed class TwitchWebhooks : ITwitchWebhooks
    {
        public TwitchWebhooks(
            IWebSubSubscription<TwitchWebhooksContentWrapperDto<TwitchStreamChangedDto>> streamsWebSubSubscription =
                default)
        {
            streamsWebSubSubscription.ContentAvailable += async (sender, e) =>
            {
                if (e.Content.Data.Any())
                {
                    var stream = e.Content.Data.First();
                    if (StreamOnline != null)
                        await StreamOnline.InvokeAsync(
                            this,
                            new TwitchStreamOnlineEventArgs(
                                stream.UserId,
                                stream.Title));
                }
                else
                {
                    if (StreamOffline != null)
                        await StreamOffline.InvokeAsync(
                            this,
                            new TwitchStreamOfflineEventArgs(
                                TwitchWebhooksTopics.GetUserIdFromStreamChanged(e.Topic)));
                }
            };
        }

        public event AsyncEventHandler<TwitchStreamOnlineEventArgs> StreamOnline;
        public event AsyncEventHandler<TwitchStreamOfflineEventArgs> StreamOffline;
    }
}