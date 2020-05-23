using System;
using Microsoft.Extensions.Configuration;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder AddElasticVoiceChannels(
            this Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder,
            Action<ElasticVoiceChannelsOptions> configureOptions)
        {
            return moduleBuilder
                .AddModule<ElasticVoiceChannelDiscordModule, ElasticVoiceChannelsOptions>(configureOptions);
        }

        public static Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder AddElasticVoiceChannels(
            this Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder,
            IConfiguration configuration)
        {
            return moduleBuilder
                .AddModule<ElasticVoiceChannelDiscordModule, ElasticVoiceChannelsOptions>(configuration);
        }
    }
}