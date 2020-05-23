using Microsoft.AspNetCore.Builder;
using TrillBot.WebSub.Extensions;

namespace TrillBot.Twitch.Webhooks.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTwitchWebhooks(this IApplicationBuilder app)
        {
            return app.UseWebSub();
        }
    }
}