using System.Collections.Generic;

namespace TrillBot.Discord.Modules.Options
{
    public class ModulesOptions
    {
        public const string Name = "Modules";

        public IEnumerable<ulong> LogChannelIds { get; set; }
    }
}