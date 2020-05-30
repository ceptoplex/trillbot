using System.Collections.Generic;
using TrillBot.Discord.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels.Options
{
    public sealed class DiscordElasticVoiceChannelsModuleOptions : DiscordModuleOptions
    {
        public new static readonly string Key = $"{DiscordModuleOptions.Key}:ElasticVoiceChannels";

        public IEnumerable<ulong> ExcludedChannelIds { get; set; }
        public int? MaxGroupChannelCount { get; set; }
    }
}