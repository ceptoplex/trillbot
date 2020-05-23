using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrillBot.WebSub.Filters;

namespace TrillBot.WebSub.Controllers
{
    [ApiController]
    internal sealed class ContentCallbackController<TContent> : ControllerBase
    {
        private readonly IContentCallback<TContent> _contentCallback;

        public ContentCallbackController(IContentCallback<TContent> contentCallback)
        {
            _contentCallback = contentCallback;
        }

        [HttpPost]
        [Route("{subscriptionMetadataId}")]
        [VerifySignature]
        [VerifyRequestDeduplication]
        public async Task<IActionResult> PostAsync(
            [FromRoute] Guid subscriptionMetadataId,
            [FromBody] TContent content,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _contentCallback.HandleContentAsync(
                    subscriptionMetadataId,
                    content,
                    cancellationToken);
            }
            catch (InvalidSubscriptionCallbackException)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}