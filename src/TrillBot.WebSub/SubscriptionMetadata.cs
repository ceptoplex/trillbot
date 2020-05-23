using System;

namespace TrillBot.WebSub
{
    internal sealed class SubscriptionMetadata : IWebSubSubscriptionMetadata
    {
        public SubscriptionMetadata(
            Uri topic,
            Type contentType,
            TimeSpan lease)
        {
            Topic = topic;
            ContentType = contentType;
            Lease = lease;
        }

        public Uri Topic { get; }
        public Type ContentType { get; }
        public TimeSpan Lease { get; }
        public Guid? CurrentId { get; private set; }
        public DateTime? CurrentLeaseExpiration { get; private set; }
        public WebSubSubscriptionState State { get; private set; } = WebSubSubscriptionState.SubscriptionNotRequested;

        public void HandleSubscriptionRequest(Guid? existingId = default, DateTime? existingLeaseExpiration = default)
        {
            CurrentId = existingId ?? Guid.NewGuid();
            CurrentLeaseExpiration = existingLeaseExpiration ?? DateTime.UtcNow + Lease;
            State = WebSubSubscriptionState.SubscriptionRequested;
        }

        public void HandleUnsubscriptionRequest()
        {
            if (State == WebSubSubscriptionState.SubscriptionNotRequested)
                throw new InvalidOperationException("Cannot perform unsubscription before initial subscription.");

            State = WebSubSubscriptionState.UnsubscriptionRequested;
        }

        public void HandleVerification(bool successful)
        {
            State = State switch
            {
                WebSubSubscriptionState.SubscriptionRequested => successful
                    ? WebSubSubscriptionState.SubscriptionSuccessful
                    : WebSubSubscriptionState.SubscriptionFailed,
                WebSubSubscriptionState.UnsubscriptionRequested => successful
                    ? WebSubSubscriptionState.UnsubscriptionSuccessful
                    : WebSubSubscriptionState.UnsubscriptionFailed,
                _ => throw new InvalidOperationException("Neither subscription nor unsubscription have been requested.")
            };
        }
    }
}