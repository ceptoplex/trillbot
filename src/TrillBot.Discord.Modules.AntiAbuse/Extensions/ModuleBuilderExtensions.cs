namespace TrillBot.Discord.Modules.AntiAbuse.Extensions
{
    public static class ModuleBuilderExtensions
    {
        public static Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder AddAntiAbuse(
            this Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.AddModule<DiscordAntiAbuseModule>();
        }
    }
}