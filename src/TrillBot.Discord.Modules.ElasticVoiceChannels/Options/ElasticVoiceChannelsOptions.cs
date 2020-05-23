using System.Collections.Generic;
using TrillBot.Discord.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Options
{
    public class ElasticVoiceChannelsOptions : ModuleOptions
    {
        public new static readonly string Key = $"{ModuleOptions.Key}:ElasticVoiceChannels";

        public IEnumerable<ulong> ExcludedChannelIds { get; set; }
        public int? MaxGroupChannelCount { get; set; }
    }
}