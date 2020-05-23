using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TrillBot.Twitch.Api.Authentication;
using TrillBot.Twitch.Api.Options;

namespace TrillBot.Twitch.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTwitchApi(
            this IServiceCollection services,
            Action<TwitchApiOptions> configureOptions)
        {
            return services
                .AddTwitchApiDependencies()
                .Configure(configureOptions);
        }

        public static IServiceCollection AddTwitchApi(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .AddTwitchApiDependencies()
                .Configure<TwitchApiOptions>(configuration);
        }

        private static IServiceCollection AddTwitchApiDependencies(
            this IServiceCollection services)
        {
            services.TryAddSingleton<ITwitchAppAccessTokenProvider, TwitchCachedAppAccessTokenProvider>();
            services.TryAddSingleton<ITwitchUnauthenticatedApiClient>(_ =>
            {
                var options = _.GetRequiredService<IOptions<TwitchApiOptions>>().Value;
                return new TwitchUnauthenticatedApiClient(
                    options.ClientId,
                    options.ClientSecret);
            });
            services.TryAddSingleton<ITwitchApiClient>(_ =>
            {
                var options = _.GetRequiredService<IOptions<TwitchApiOptions>>().Value;
                return new TwitchApiClient(
                    _.GetRequiredService<ITwitchAppAccessTokenProvider>(),
                    options.ClientId,
                    options.ClientSecret);
            });

            return services;
        }
    }
}