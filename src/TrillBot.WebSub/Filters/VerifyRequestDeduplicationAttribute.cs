using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace TrillBot.WebSub.Filters
{
    internal sealed class VerifyRequestDeduplicationAttribute : ActionFilterAttribute
    {
        // TODO: This should probably be persisted or have a longer lifetime than the application.
        private readonly ICollection<object> _deduplicationIds = new List<object>();

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;

            var contentDeduplicationIdResolver =
                httpContext.RequestServices.GetService<IWebSubContentDeduplicationIdResolver>();
            if (contentDeduplicationIdResolver == null)
            {
                await next();
                return;
            }

            if (!await contentDeduplicationIdResolver.TryResolveContentDeduplicationIdAsync(httpContext,
                out var deduplicationId))
            {
                context.ModelState.AddModelError(string.Empty, "Content deduplication ID is missing.");
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            if (HasReceivedContentAlready(deduplicationId))
            {
                context.Result = new OkResult();
                return;
            }

            await next();
        }

        private bool HasReceivedContentAlready(object deduplicationId)
        {
            if (_deduplicationIds.Contains(deduplicationId))
                return true;
            _deduplicationIds.Add(deduplicationId);
            return false;
        }
    }
}