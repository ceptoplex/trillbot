using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticVoiceChannelsModule(
            this IServiceCollection services,
            Action<ElasticVoiceChannelsOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<IModule, ElasticVoiceChannelModule>();

            return services;
        }

        public static IServiceCollection AddElasticVoiceChannelsModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ElasticVoiceChannelsOptions>(configuration);
            services.AddSingleton<IModule, ElasticVoiceChannelModule>();

            return services;
        }
    }
}