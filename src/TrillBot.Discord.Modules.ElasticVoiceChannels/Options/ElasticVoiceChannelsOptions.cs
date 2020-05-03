using System.Collections.Generic;
using TrillBot.Discord.Modules.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Options
{
    public class ElasticVoiceChannelsOptions : IModuleOptions
    {
        public const string Name = "ElasticVoiceChannels";

        public IEnumerable<ulong> ExcludedChannelIds { get; set; }
        public int? MaxGroupChannelCount { get; set; }
    }
}