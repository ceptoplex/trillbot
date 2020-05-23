using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TrillBot.WebSub;

namespace TrillBot.Twitch.Webhooks
{
    internal sealed class TwitchWebhooksWebSubContentDeduplicationIdResolver : IWebSubContentDeduplicationIdResolver
    {
        public Task<bool> TryResolveContentDeduplicationIdAsync(
            HttpContext context,
            out object deduplicationId,
            CancellationToken cancellationToken = default)
        {
            var successful = context.Request.Headers.TryGetValue(
                "Twitch-Notification-Id",
                out var deduplicationIdStringValues);
            deduplicationId = (string) deduplicationIdStringValues;
            return Task.FromResult(successful);
        }
    }
}