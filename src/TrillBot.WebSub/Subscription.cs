using TrillBot.Common.Events;
using TrillBot.Common.Extensions;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub
{
    internal sealed class Subscription<TContent> : IWebSubSubscription<TContent>
    {
        public Subscription(IContentCallback<TContent> contentCallback)
        {
            contentCallback.ContentReceived += async (sender, e) =>
            {
                if (ContentAvailable != null)
                    await ContentAvailable.InvokeAsync(
                        this,
                        new WebSubContentAvailableEventArgs<TContent>(
                            e.SubscriptionMetadata.Topic,
                            e.Content),
                        e.CancellationToken);
            };
        }

        public event AsyncEventHandler<WebSubContentAvailableEventArgs<TContent>> ContentAvailable;
    }
}