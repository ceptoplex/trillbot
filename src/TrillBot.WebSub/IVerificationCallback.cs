using System;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Common.Events;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub
{
    internal interface IVerificationCallback
    {
        event AsyncEventHandler<VerificationReceivedEventArgs> VerificationReceived;

        Task HandleVerificationAsync(
            Guid contentTypeId,
            Guid subscriptionMetadataId,
            Uri topic,
            SubscriptionMode mode,
            TimeSpan? lease,
            string reason,
            CancellationToken cancellationToken = default);
    }
}