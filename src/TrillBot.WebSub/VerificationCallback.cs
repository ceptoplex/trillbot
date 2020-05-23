using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Common.Events;
using TrillBot.Common.Extensions;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub
{
    internal sealed class VerificationCallback : Callback, IVerificationCallback
    {
        public VerificationCallback(
            IEnumerable<IWebSubSubscriptionMetadata> subscriptionMetadata) : base(
            subscriptionMetadata)
        {
        }

        public event AsyncEventHandler<VerificationReceivedEventArgs> VerificationReceived;

        public async Task HandleVerificationAsync(
            Guid contentTypeId,
            Guid subscriptionMetadataId,
            Uri topic,
            SubscriptionMode mode,
            TimeSpan? lease,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var subscriptionMetadata = GetSubscriptionMetadata(contentTypeId, subscriptionMetadataId);

            if (topic != subscriptionMetadata.Topic ||
                mode == SubscriptionMode.Subscribe && lease != subscriptionMetadata.Lease)
                throw new InvalidSubscriptionCallbackException();

            if (mode == SubscriptionMode.Denied)
            {
                subscriptionMetadata.HandleVerification(false);
            }
            else
            {
                if (mode == SubscriptionMode.Subscribe &&
                    subscriptionMetadata.State != WebSubSubscriptionState.SubscriptionRequested ||
                    mode == SubscriptionMode.Unsubscribe &&
                    subscriptionMetadata.State != WebSubSubscriptionState.UnsubscriptionRequested)
                    throw new InvalidSubscriptionCallbackException();

                switch (mode)
                {
                    case SubscriptionMode.Subscribe:
                    case SubscriptionMode.Unsubscribe:
                        subscriptionMetadata.HandleVerification(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            if (VerificationReceived != null)
                await VerificationReceived.InvokeAsync(
                    this,
                    new VerificationReceivedEventArgs(mode, subscriptionMetadata),
                    cancellationToken);
        }
    }
}