using System;

namespace TrillBot.WebSub
{
    public interface IWebSubSubscriptionMetadata
    {
        Uri Topic { get; }
        Type ContentType { get; }
        TimeSpan Lease { get; }
        Guid? CurrentId { get; }
        DateTime? CurrentLeaseExpiration { get; }
        WebSubSubscriptionState State { get; }

        void HandleSubscriptionRequest(Guid? existingId = default, DateTime? existingLeaseExpiration = default);
        void HandleUnsubscriptionRequest();
        void HandleVerification(bool successful);
    }
}