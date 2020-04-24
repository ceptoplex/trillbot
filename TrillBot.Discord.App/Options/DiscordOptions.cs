using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.App.Options
{
    internal class DiscordOptions
    {
        public const string Name = "Discord";

        public string Token { get; set; }
        public ModulesOptions Modules { get; set; }

        public class ModulesOptions : Modules.Options.ModulesOptions
        {
            public ElasticVoiceChannelsOptions ElasticVoiceChannels { get; set; }
        }
    }
}