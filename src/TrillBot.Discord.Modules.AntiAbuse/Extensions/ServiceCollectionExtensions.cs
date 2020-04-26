using Microsoft.Extensions.DependencyInjection;

namespace TrillBot.Discord.Modules.AntiAbuse.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAntiAbuseDiscordModule(this IServiceCollection services)
        {
            services.AddSingleton<IModule, AntiAbuseModule>();

            return services;
        }
    }
}