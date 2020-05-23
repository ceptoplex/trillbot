using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Common.Events;
using TrillBot.Common.Extensions;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub
{
    internal sealed class ContentCallback<TContent> : Callback, IContentCallback<TContent>
    {
        public ContentCallback(
            IEnumerable<IWebSubSubscriptionMetadata> subscriptionMetadata) : base(
            subscriptionMetadata)
        {
        }

        public event AsyncEventHandler<ContentReceivedEventArgs<TContent>> ContentReceived;

        public async Task HandleContentAsync(
            Guid subscriptionMetadataId,
            TContent content,
            CancellationToken cancellationToken = default)
        {
            var subscriptionMetadata = GetSubscriptionMetadata(typeof(TContent).GUID, subscriptionMetadataId);

            if (ContentReceived != null)
                await ContentReceived.InvokeAsync(
                    this,
                    new ContentReceivedEventArgs<TContent>(
                        subscriptionMetadata,
                        content),
                    cancellationToken);
        }
    }
}