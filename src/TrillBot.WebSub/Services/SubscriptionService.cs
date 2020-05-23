using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TrillBot.Common.Extensions;
using TrillBot.WebSub.Api;
using TrillBot.WebSub.Authentication;
using TrillBot.WebSub.EventArgs;

namespace TrillBot.WebSub.Services
{
    internal sealed class SubscriptionService : BackgroundService
    {
        private readonly Uri _callbackPublicUrl;
        private readonly IHubApiClient _hubApiClient;
        private readonly IKeyVault _keyVault;
        private readonly TimeSpan _leaseRenewalBuffer;
        private readonly IEnumerable<IWebSubSubscriptionMetadata> _subscriptionMetadata;
        private readonly IWebSubSubscriptionMetadataResolver _subscriptionMetadataResolver;
        private readonly IVerificationCallback _verificationCallback;

        public SubscriptionService(
            IEnumerable<IWebSubSubscriptionMetadata> subscriptionMetadata,
            IVerificationCallback verificationCallback,
            IKeyVault keyVault,
            IHubApiClient hubApiClient,
            Uri callbackPublicUrl,
            TimeSpan leaseRenewalBuffer,
            IWebSubSubscriptionMetadataResolver subscriptionMetadataResolver = default)
        {
            _subscriptionMetadata = subscriptionMetadata;
            _verificationCallback = verificationCallback;
            _keyVault = keyVault;
            _hubApiClient = hubApiClient;
            _callbackPublicUrl = callbackPublicUrl;
            _leaseRenewalBuffer = leaseRenewalBuffer;
            _subscriptionMetadataResolver = subscriptionMetadataResolver;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_subscriptionMetadata.Any())
                return;

            await UnsubscribeAllExistingAsync(stoppingToken);
            await SubscribeAllForeverAsync(stoppingToken);
        }

        private async Task UnsubscribeAllExistingAsync(CancellationToken cancellationToken = default)
        {
            foreach (var subscriptionMetadata in _subscriptionMetadata)
            {
                await _subscriptionMetadataResolver.UpdateSubscriptionMetadataAsync(
                    subscriptionMetadata,
                    cancellationToken);
                if (subscriptionMetadata.State == WebSubSubscriptionState.SubscriptionSuccessful)
                    await UnsubscribeAsync(subscriptionMetadata, cancellationToken);
            }
        }

        private async Task SubscribeAllForeverAsync(CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(_subscriptionMetadata.Select(_ => SubscribeAsync(_, cancellationToken)));

            while (!cancellationToken.IsCancellationRequested)
            {
                var subscriptionMetadata = _subscriptionMetadata
                    .Where(_ => _.CurrentLeaseExpiration.HasValue)
                    .OrderBy(_ => _.CurrentLeaseExpiration.Value).First();
                var untilLeaseRenewal =
                    subscriptionMetadata.CurrentLeaseExpiration!.Value -
                    DateTime.UtcNow -
                    _leaseRenewalBuffer;
                if (untilLeaseRenewal.TotalMilliseconds > 0)
                    await Task.Delay((int) Math.Floor(untilLeaseRenewal.TotalMilliseconds), cancellationToken);

                await SubscribeAsync(subscriptionMetadata, cancellationToken);
            }
        }

        private async Task SubscribeAsync(
            IWebSubSubscriptionMetadata subscriptionMetadata,
            CancellationToken cancellationToken = default)
        {
            subscriptionMetadata.HandleSubscriptionRequest();

            await AwaitVerificationAsync(
                subscriptionMetadata,
                SubscriptionMode.Subscribe,
                async () =>
                    await _hubApiClient.Hub().SubscribeAsync(
                        CreateCallbackUrl(subscriptionMetadata),
                        subscriptionMetadata.Topic,
                        subscriptionMetadata.Lease,
                        _keyVault.GetKey(),
                        cancellationToken));
        }

        private async Task UnsubscribeAsync(
            IWebSubSubscriptionMetadata subscriptionMetadata,
            CancellationToken cancellationToken = default)
        {
            try
            {
                subscriptionMetadata.HandleUnsubscriptionRequest();
            }
            catch (InvalidOperationException)
            {
                // Has never been subscribed, therefore unsubscription is unnecessary.
                return;
            }

            await AwaitVerificationAsync(
                subscriptionMetadata,
                SubscriptionMode.Unsubscribe,
                async () =>
                    await _hubApiClient.Hub().UnsubscribeAsync(
                        CreateCallbackUrl(subscriptionMetadata),
                        subscriptionMetadata.Topic,
                        cancellationToken));
        }

        private Uri CreateCallbackUrl(IWebSubSubscriptionMetadata subscriptionMetadata)
        {
            return _callbackPublicUrl
                .Append(subscriptionMetadata.ContentType.GUID.ToString())
                .Append(subscriptionMetadata.CurrentId.ToString());
        }

        private async Task AwaitVerificationAsync(
            IWebSubSubscriptionMetadata subscriptionMetadata,
            SubscriptionMode subscriptionMode,
            Func<Task> function)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _verificationCallback.VerificationReceived += OnVerificationReceived;

            await function();

            await taskCompletionSource.Task;
            _verificationCallback.VerificationReceived -= OnVerificationReceived;

            Task OnVerificationReceived(object sender, VerificationReceivedEventArgs e)
            {
                if (e.SubscriptionMode == SubscriptionMode.Denied)
                    throw new Exception("Subscription or unsubscription process failed.");

                if (e.SubscriptionMetadata != subscriptionMetadata ||
                    e.SubscriptionMode != subscriptionMode)
                    return Task.CompletedTask;
                // TODO: This is not sufficient because the verification callback callback result may get lost. 
                taskCompletionSource.SetResult(true);
                return Task.CompletedTask;
            }
        }
    }
}