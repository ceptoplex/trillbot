using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TrillBot.Twitch.Api;
using TrillBot.Twitch.Api.Authentication;
using TrillBot.Twitch.Api.Options;
using TrillBot.Twitch.Webhooks.Api;
using TrillBot.Twitch.Webhooks.Entities;
using TrillBot.Twitch.Webhooks.Options;
using TrillBot.WebSub;
using TrillBot.WebSub.Extensions;
using TrillBot.WebSub.Options;

namespace TrillBot.Twitch.Webhooks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Uri HubUrl = new Uri("https://api.twitch.tv/helix/webhooks/hub");

        // According to Twitch, the maximum lease duration is 10 days,
        // so 1 week (= 7 days) is chosen here, with a renewal buffer of 1 day.
        // See: https://dev.twitch.tv/docs/api/webhooks-reference#subscribe-tounsubscribe-from-events
        private static readonly TimeSpan Lease = TimeSpan.FromDays(7);
        private static readonly TimeSpan LeaseRenewalBuffer = TimeSpan.FromDays(1);

        public static IServiceCollection AddTwitchWebhooks(
            this IServiceCollection services,
            string callbackControllerRouteTemplate,
            Action<TopicsBuilder> configureTopics,
            Action<TwitchWebhooksOptions> configureOptions)
        {
            return services
                .AddTwitchWebhooksDependencies()
                .AddWebSub(
                    HubUrl,
                    callbackControllerRouteTemplate,
                    options => configureTopics(new TopicsBuilder(options)),
                    options => TranslateOptions(options, configureOptions))
                .Configure(configureOptions);
        }

        public static IServiceCollection AddTwitchWebhooks(
            this IServiceCollection services,
            string callbackControllerRouteTemplate,
            Action<TopicsBuilder> configureTopics,
            IConfiguration configuration)
        {
            return services
                .AddTwitchWebhooksDependencies()
                .AddWebSub(
                    HubUrl,
                    callbackControllerRouteTemplate,
                    options => configureTopics(new TopicsBuilder(options)),
                    options => TranslateOptions(options, configuration.Get<TwitchWebhooksOptions>()))
                .Configure<TwitchWebhooksOptions>(configuration);
        }

        private static IServiceCollection AddTwitchWebhooksDependencies(
            this IServiceCollection services)
        {
            if (services.All(_ => _.ServiceType != typeof(ITwitchApiClient)) ||
                services.All(_ => _.ServiceType != typeof(ITwitchAppAccessTokenProvider)))
                throw new InvalidOperationException(
                    $"Before adding {nameof(TrillBot)}.{nameof(Twitch)}.{nameof(Webhooks)} services, " +
                    $"please add {nameof(TrillBot)}.{nameof(Twitch)}.{nameof(Api)} services at first.");

            services.TryAddSingleton<
                IWebSubSubscriptionMetadataResolver,
                TwitchWebhooksWebSubSubscriptionMetadataResolver
            >();
            services.TryAddSingleton<
                IWebSubContentDeduplicationIdResolver,
                TwitchWebhooksWebSubContentDeduplicationIdResolver
            >();
            services.TryAddSingleton<ITwitchWebhooks, TwitchWebhooks>();

            return services;
        }

        private static void TranslateOptions(
            WebSubOptions webSubOptions,
            Action<TwitchWebhooksOptions> configureOptions)
        {
            var webhooksOptions = new TwitchWebhooksOptions();
            configureOptions(webhooksOptions);
            TranslateOptions(webSubOptions, webhooksOptions);
        }

        private static void TranslateOptions(
            WebSubOptions webSubOptions,
            TwitchWebhooksOptions webhooksOptions)
        {
            webSubOptions.CallbackPublicUrl = webhooksOptions.CallbackPublicUrl;
            webSubOptions.Lease = Lease;
            webSubOptions.LeaseRenewalBuffer = LeaseRenewalBuffer;
        }

        public class TopicsBuilder
        {
            private readonly WebSub.Extensions.ServiceCollectionExtensions.SubscriptionsBuilder _subscriptionsBuilder;

            public TopicsBuilder(
                WebSub.Extensions.ServiceCollectionExtensions.SubscriptionsBuilder subscriptionsBuilder)
            {
                _subscriptionsBuilder = subscriptionsBuilder;

                subscriptionsBuilder.AddHubAuthenticator(_ =>
                    new TwitchWebSubHubApiRequestAuthenticator(
                        _.GetRequiredService<ITwitchAppAccessTokenProvider>(),
                        _.GetRequiredService<IOptions<TwitchApiOptions>>().Value.ClientId));
            }

            public TopicsBuilder AddStreams(string userId)
            {
                _subscriptionsBuilder
                    .AddSubscription<TwitchWebhooksContentWrapperDto<TwitchStreamChangedDto>>(
                        TwitchWebhooksTopics.CreateStreamChanged(userId));

                return this;
            }
        }
    }
}