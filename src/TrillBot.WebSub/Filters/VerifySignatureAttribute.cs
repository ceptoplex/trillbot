using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Common;
using TrillBot.WebSub.Authentication;

namespace TrillBot.WebSub.Filters
{
    internal sealed class VerifySignatureAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var signature = GetSignature(context);
            var requestBody = await GetRequestBodyAsync(context);
            var signatureVerifier = context.HttpContext.RequestServices.GetRequiredService<ISignatureVerifier>();

            if (!signatureVerifier.VerifySignature(signature, requestBody))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        private static IEnumerable<byte> GetSignature(ActionContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Hub-Signature", out var signatureString))
                return null;
            var signatureMatchedGroup = Regex
                .Match(signatureString, "^sha256=(?<signature>[a-f0-9]{64})$")
                .Groups["signature"];
            return signatureMatchedGroup.Success
                ? ConversionUtil.FromHexString(signatureMatchedGroup.Value)
                : null;
        }

        private static async Task<IEnumerable<byte>> GetRequestBodyAsync(
            ActionContext context,
            CancellationToken cancellationToken = default)
        {
            var body = context.HttpContext.Request.Body;

            var oldBodyPosition = body.Position;
            body.Position = 0;

            await using var rawContent = new MemoryStream();
            await body.CopyToAsync(rawContent, cancellationToken);

            body.Position = oldBodyPosition;

            return rawContent.ToArray();
        }
    }
}