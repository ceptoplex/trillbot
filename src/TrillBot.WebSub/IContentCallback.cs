using System;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Common.Events;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub
{
    internal interface IContentCallback<TContent>
    {
        event AsyncEventHandler<ContentReceivedEventArgs<TContent>> ContentReceived;

        Task HandleContentAsync(
            Guid subscriptionMetadataId,
            TContent content,
            CancellationToken cancellationToken = default);
    }
}