namespace TrillBot.Discord.Modules.Ping.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder AddPing(
            this Discord.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.AddModule<PingDiscordModule>();
        }
    }
}