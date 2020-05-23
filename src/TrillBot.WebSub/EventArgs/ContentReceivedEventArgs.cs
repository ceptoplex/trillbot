using TrillBot.Common.Events;

namespace TrillBot.WebSub.EventArgs
{
    internal sealed class ContentReceivedEventArgs<TContent> : CancelableEventArgs
    {
        public ContentReceivedEventArgs(
            IWebSubSubscriptionMetadata subscriptionMetadata,
            TContent content)
        {
            SubscriptionMetadata = subscriptionMetadata;
            Content = content;
        }

        public IWebSubSubscriptionMetadata SubscriptionMetadata { get; }
        public TContent Content { get; }
    }
}