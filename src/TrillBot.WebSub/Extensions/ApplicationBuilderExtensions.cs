using Microsoft.AspNetCore.Builder;
using TrillBot.WebSub.Middleware;

namespace TrillBot.WebSub.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebSub(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestBodyBufferingMiddleware>();
        }
    }
}