using System.Collections.Generic;

namespace TrillBot.Discord.Options
{
    public class DiscordOptions
    {
        public const string Name = "Discord";

        public string Token { get; set; }
        public IEnumerable<ulong> LogChannelIds { get; set; }
    }
}