using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.Discord.Extensions
{
    internal static class CancellationTokenExtensions
    {
        public static Task AwaitCanceled(this CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => taskCompletionSource.SetResult(true));
            return taskCompletionSource.Task;
        }
    }
}