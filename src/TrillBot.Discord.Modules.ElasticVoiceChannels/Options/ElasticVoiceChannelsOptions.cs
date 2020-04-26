using System.Collections.Generic;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Options
{
    public class ElasticVoiceChannelsOptions
    {
        public const string Name = "ElasticVoiceChannels";

        public IEnumerable<ulong> ExcludedChannelIds { get; set; }
        public int? MaxGroupChannelCount { get; set; }
    }
}