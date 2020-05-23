using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrillBot.Common.Events;

namespace TrillBot.Common.Extensions
{
    public static class AsyncEventHandlerExtensions
    {
        public static Task InvokeAsync(
            this AsyncEventHandler asyncEventHandler,
            object sender,
            CancelableEventArgs e,
            CancellationToken cancellationToken = default)
        {
            e.CancellationToken = cancellationToken;
            return Task.WhenAll(
                asyncEventHandler
                    .GetInvocationList()
                    .Cast<AsyncEventHandler>()
                    .Select(_ => _(sender, e)));
        }

        public static Task InvokeAsync<TCancelableEventArgs>(
            this AsyncEventHandler<TCancelableEventArgs> asyncEventHandler,
            object sender,
            TCancelableEventArgs e,
            CancellationToken cancellationToken = default)
            where TCancelableEventArgs : CancelableEventArgs
        {
            e.CancellationToken = cancellationToken;
            return Task.WhenAll(
                asyncEventHandler
                    .GetInvocationList()
                    .Cast<AsyncEventHandler<TCancelableEventArgs>>()
                    .Select(_ => _(sender, e)));
        }
    }
}