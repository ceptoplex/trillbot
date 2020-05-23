using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrillBot.WebSub.Entities;

namespace TrillBot.WebSub.Controllers
{
    [ApiController]
    internal sealed class VerificationCallbackController : ControllerBase
    {
        private readonly IVerificationCallback _verificationCallback;

        public VerificationCallbackController(IVerificationCallback verificationCallback)
        {
            _verificationCallback = verificationCallback;
        }

        [HttpGet]
        [Route("{contentTypeId}/{subscriptionMetadataId}")]
        public async Task<IActionResult> GetAsync(
            [FromRoute] Guid contentTypeId,
            [FromRoute] Guid subscriptionMetadataId,
            [FromQuery] VerificationDto verification,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _verificationCallback.HandleVerificationAsync(
                    contentTypeId,
                    subscriptionMetadataId,
                    verification.Topic,
                    verification.Mode,
                    verification.Lease,
                    verification.Reason,
                    cancellationToken);
            }
            catch (InvalidSubscriptionCallbackException)
            {
                return NotFound();
            }

            return Ok(verification.Challenge);
        }
    }
}