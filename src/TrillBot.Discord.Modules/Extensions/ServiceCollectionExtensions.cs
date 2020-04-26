using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Modules.Options;

namespace TrillBot.Discord.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscordModules(
            this IServiceCollection services,
            Action<ModulesOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddDiscordModules();

            return services;
        }

        public static IServiceCollection AddDiscordModules(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ModulesOptions>(configuration);
            services.AddDiscordModules();

            return services;
        }

        private static IServiceCollection AddDiscordModules(
            this IServiceCollection services)
        {
            services.AddSingleton<GuildUserAvailability>();
            services.AddSingleton<Messaging>();

            return services;
        }
    }
}