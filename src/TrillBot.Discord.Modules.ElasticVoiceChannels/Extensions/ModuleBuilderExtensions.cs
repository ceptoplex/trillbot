using System;
using Microsoft.Extensions.Configuration;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions
{
    public static class ModuleBuilderExtensions
    {
        public static Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder AddElasticVoiceChannels(
            this Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder,
            Action<DiscordElasticVoiceChannelsModuleOptions> configureOptions)
        {
            return moduleBuilder
                .AddModule<DiscordElasticVoiceChannelModule, DiscordElasticVoiceChannelsModuleOptions>(configureOptions);
        }

        public static Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder AddElasticVoiceChannels(
            this Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder,
            IConfiguration configuration)
        {
            return moduleBuilder
                .AddModule<DiscordElasticVoiceChannelModule, DiscordElasticVoiceChannelsModuleOptions>(configuration);
        }
    }
}