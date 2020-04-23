using Microsoft.Extensions.DependencyInjection;

namespace TrillBot.Discord.Modules.Ping.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPingModule(this IServiceCollection services)
        {
            services.AddSingleton<IModule, PingModule>();

            return services;
        }
    }
}