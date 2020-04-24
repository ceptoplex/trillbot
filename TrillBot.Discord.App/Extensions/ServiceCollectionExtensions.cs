using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.App.Options;

namespace TrillBot.Discord.App.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBootstrapper(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DiscordOptions>(configuration);
            services.AddBootstrapper();

            return services;
        }

        private static IServiceCollection AddBootstrapper(
            this IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<Bootstrapper>();

            return services;
        }
    }
}