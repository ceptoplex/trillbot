using System;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Options;

namespace TrillBot.Discord.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscord(
            this IServiceCollection services,
            Action<Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder> configureModules,
            Action<DiscordOptions> configureOptions)
        {
            return services
                .AddDiscordBotDependencies(configureModules)
                .Configure(configureOptions);
        }

        public static IServiceCollection AddDiscord(
            this IServiceCollection services,
            Action<Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder> configureModules,
            IConfiguration configuration)
        {
            return services
                .AddDiscordBotDependencies(configureModules)
                .Configure<DiscordOptions>(configuration);
        }

        private static IServiceCollection AddDiscordBotDependencies(
            this IServiceCollection services,
            Action<Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder> configureModules)
        {
            services
                .AddSingleton<DiscordBot>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<GuildUserAvailability>()
                .AddSingleton<Messaging>();

            var moduleBuilder = new Modules.Extensions.ServiceCollectionExtensions.ModuleBuilder(services);
            configureModules(moduleBuilder);

            return services;
        }
    }
}