using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TrillBot.WebSub.Api;
using TrillBot.WebSub.Authentication;
using TrillBot.WebSub.Options;
using TrillBot.WebSub.Services;

namespace TrillBot.WebSub.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IDictionary<IServiceCollection, SubscriptionsBuilder> SubscriptionsBuilders =
            new Dictionary<IServiceCollection, SubscriptionsBuilder>();

        public static IServiceCollection AddWebSub(
            this IServiceCollection services,
            Uri hubUrl,
            string callbackControllerRouteTemplate,
            Action<SubscriptionsBuilder> configureSubscriptions,
            Action<WebSubOptions> configureOptions)
        {
            return services
                .AddWebSubDependencies(hubUrl, callbackControllerRouteTemplate, configureSubscriptions)
                .Configure(configureOptions);
        }

        public static IServiceCollection AddWebSub(
            this IServiceCollection services,
            Uri hubUrl,
            string callbackControllerRouteTemplate,
            Action<SubscriptionsBuilder> configureSubscriptions,
            IConfiguration configuration)
        {
            return services
                .AddWebSubDependencies(hubUrl, callbackControllerRouteTemplate, configureSubscriptions)
                .Configure<WebSubOptions>(configuration);
        }

        private static IServiceCollection AddWebSubDependencies(
            this IServiceCollection services,
            Uri hubUrl,
            string callbackControllerRouteTemplate,
            Action<SubscriptionsBuilder> configureSubscriptions)
        {
            if (!SubscriptionsBuilders.ContainsKey(services))
                SubscriptionsBuilders[services] = new SubscriptionsBuilder(
                    services,
                    hubUrl,
                    callbackControllerRouteTemplate);
            configureSubscriptions(SubscriptionsBuilders[services]);

            return services;
        }

        public class SubscriptionsBuilder
        {
            private readonly IServiceCollection _services;

            private readonly ICollection<ContentTypeInfo>
                _subscriptionContentTypeInfos = new HashSet<ContentTypeInfo>();

            private readonly ICollection<Uri> _topics = new HashSet<Uri>();

            public SubscriptionsBuilder(IServiceCollection services, Uri hubUrl, string callbackControllerRouteTemplate)
            {
                _services = services;

                services
                    .AddSingleton<IKeyVault, KeyVault>()
                    .AddSingleton<ISignatureVerifier, SignatureVerifier>()
                    .AddSingleton<IHubApiClient>(_ => new HubApiClient(
                        hubUrl,
                        _.GetService<IWebSubHubApiRequestAuthenticator>()))
                    .AddSingleton<IVerificationCallback, VerificationCallback>();

                services
                    .AddMvc(options =>
                        options.Conventions.Add(
                            new CallbackControllerConvention(callbackControllerRouteTemplate)))
                    .ConfigureApplicationPartManager(options =>
                    {
                        options.FeatureProviders.Add(
                            new CallbackControllerFeatureProvider(_subscriptionContentTypeInfos));
                    });
                services
                    .AddSingleton<IHostedService>(_ =>
                    {
                        var options = _.GetRequiredService<IOptions<WebSubOptions>>().Value;
                        return new SubscriptionService(
                            _.GetServices<IWebSubSubscriptionMetadata>(),
                            _.GetRequiredService<IVerificationCallback>(),
                            _.GetRequiredService<IKeyVault>(),
                            _.GetRequiredService<IHubApiClient>(),
                            options.CallbackPublicUrl,
                            options.LeaseRenewalBuffer!.Value,
                            _.GetService<IWebSubSubscriptionMetadataResolver>());
                    });
            }

            public SubscriptionsBuilder AddSubscription<TContent>(Uri topic)
            {
                if (_topics.Contains(topic))
                    throw new InvalidOperationException("There can only be one subscription to a specific topic.");
                _topics.Add(topic);

                var subscriptionContentTypeInfo = new ContentTypeInfo(typeof(TContent));
                if (!_subscriptionContentTypeInfos.Contains(subscriptionContentTypeInfo))
                    _subscriptionContentTypeInfos.Add(subscriptionContentTypeInfo);

                _services.TryAddSingleton<IContentCallback<TContent>, ContentCallback<TContent>>();
                _services.TryAddSingleton<IWebSubSubscription<TContent>, Subscription<TContent>>();

                _services.AddSingleton<IWebSubSubscriptionMetadata>(_ =>
                {
                    var options = _.GetRequiredService<IOptions<WebSubOptions>>().Value;
                    return new SubscriptionMetadata(
                        topic,
                        subscriptionContentTypeInfo.ContentType,
                        options.Lease!.Value
                    );
                });

                return this;
            }

            public SubscriptionsBuilder AddHubAuthenticator<THubAuthenticator>(
                Func<IServiceProvider, THubAuthenticator> implementationFactory = default)
                where THubAuthenticator : class, IWebSubHubApiRequestAuthenticator
            {
                if (implementationFactory == default)
                    _services.AddSingleton<IWebSubHubApiRequestAuthenticator, THubAuthenticator>();
                else
                    _services.AddSingleton<IWebSubHubApiRequestAuthenticator>(implementationFactory);

                return this;
            }
        }
    }
}