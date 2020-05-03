namespace TrillBot.Discord.Modules.AntiAbuse.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder AddAntiAbuse(
            this Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.AddModule<AntiAbuseModule>();
        }
    }
}