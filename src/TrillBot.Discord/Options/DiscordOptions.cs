using TrillBot.Discord.Modules.Options;

namespace TrillBot.Discord.Options
{
    public class DiscordOptions<TModulesOptions>
        where TModulesOptions : ModulesOptions
    {
        public const string Name = "Discord";

        public string Token { get; set; }
        public TModulesOptions Modules { get; set; }
    }
}