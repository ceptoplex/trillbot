using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Modules.Options;

namespace TrillBot.Discord.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModules(
            this IServiceCollection services,
            Action<ModulesOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddModules();

            return services;
        }

        public static IServiceCollection AddModules(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ModulesOptions>(configuration);
            services.AddModules();

            return services;
        }

        private static IServiceCollection AddModules(
            this IServiceCollection services)
        {
            services.AddSingleton<GuildUserAvailability>();
            services.AddSingleton<Messaging>();

            return services;
        }
    }
}