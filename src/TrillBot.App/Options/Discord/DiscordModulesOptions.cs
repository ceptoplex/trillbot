using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;
using TrillBot.Discord.Modules.Options;

namespace TrillBot.App.Options.Discord
{
    internal class DiscordModulesOptions : ModulesOptions
    {
        public ElasticVoiceChannelsOptions ElasticVoiceChannels { get; set; }
    }
}