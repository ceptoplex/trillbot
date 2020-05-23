using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.WebSub
{
    public interface IWebSubSubscriptionMetadataResolver
    {
        Task UpdateSubscriptionMetadataAsync(
            IWebSubSubscriptionMetadata subscriptionMetadata,
            CancellationToken cancellationToken = default);
    }
}