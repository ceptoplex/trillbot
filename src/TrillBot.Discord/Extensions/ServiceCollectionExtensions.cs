using System;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Modules.Options;
using TrillBot.Discord.Options;

namespace TrillBot.Discord.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscordBot<TModulesOptions>(
            this IServiceCollection services,
            Action<DiscordOptions<TModulesOptions>> configureOptions)
            where TModulesOptions : ModulesOptions
        {
            services.Configure(configureOptions);
            services.AddDiscordBot<TModulesOptions>();

            return services;
        }

        public static IServiceCollection AddDiscordBot<TModulesOptions>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TModulesOptions : ModulesOptions
        {
            services.Configure<DiscordOptions<TModulesOptions>>(configuration);
            services.AddDiscordBot<TModulesOptions>();

            return services;
        }

        private static IServiceCollection AddDiscordBot<TModulesOptions>(
            this IServiceCollection services)
            where TModulesOptions : ModulesOptions
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<DiscordBot<TModulesOptions>>();

            return services;
        }
    }
}