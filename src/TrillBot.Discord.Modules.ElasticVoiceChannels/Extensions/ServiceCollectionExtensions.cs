using System;
using Microsoft.Extensions.Configuration;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder AddElasticVoiceChannels(
            this Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder,
            Action<ElasticVoiceChannelsOptions> configureOptions)
        {
            return moduleBuilder.AddModule<ElasticVoiceChannelModule, ElasticVoiceChannelsOptions>(configureOptions);
        }

        public static Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder AddElasticVoiceChannels(
            this Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder,
            IConfiguration configuration)
        {
            return moduleBuilder.AddModule<ElasticVoiceChannelModule, ElasticVoiceChannelsOptions>(configuration);
        }
    }
}