using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TrillBot.WebSub.Middleware
{
    internal sealed class RequestBodyBufferingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestBodyBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            await _next(httpContext);
        }
    }
}