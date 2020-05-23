using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TrillBot.WebSub
{
    public interface IWebSubContentDeduplicationIdResolver
    {
        Task<bool> TryResolveContentDeduplicationIdAsync(
            HttpContext context,
            out object deduplicationId,
            CancellationToken cancellationToken = default);
    }
}