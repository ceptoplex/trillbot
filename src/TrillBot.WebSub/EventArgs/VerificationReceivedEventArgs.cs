using TrillBot.Common.Events;

namespace TrillBot.WebSub.EventArgs
{
    internal sealed class VerificationReceivedEventArgs : CancelableEventArgs
    {
        public VerificationReceivedEventArgs(
            SubscriptionMode subscriptionMode,
            IWebSubSubscriptionMetadata subscriptionMetadata)
        {
            SubscriptionMode = subscriptionMode;
            SubscriptionMetadata = subscriptionMetadata;
        }

        public SubscriptionMode SubscriptionMode { get; }
        public IWebSubSubscriptionMetadata SubscriptionMetadata { get; }
    }
}