namespace TrillBot.Discord.Modules.Ping.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder AddPing(
            this Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.AddModule<PingModule>();
        }
    }
}