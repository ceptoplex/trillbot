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
            Action<ModuleBuilder> configureModules,
            Action<DiscordOptions> configureOptions)
        {
            return services
                .AddDiscordBotDependencies(configureModules)
                .Configure(configureOptions);
        }

        public static IServiceCollection AddDiscord(
            this IServiceCollection services,
            Action<ModuleBuilder> configureModules,
            IConfiguration configuration)
        {
            return services
                .AddDiscordBotDependencies(configureModules)
                .Configure<DiscordOptions>(configuration);
        }

        private static IServiceCollection AddDiscordBotDependencies(
            this IServiceCollection services,
            Action<ModuleBuilder> configureModules)
        {
            services
                .AddSingleton<DiscordBot>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<DiscordGuildUserAvailability>()
                .AddSingleton<DiscordMessaging>();

            var moduleBuilder = new ModuleBuilder(services);
            configureModules(moduleBuilder);

            return services;
        }

        public class ModuleBuilder
        {
            public ModuleBuilder(IServiceCollection services)
            {
                Services = services;
            }

            public IServiceCollection Services { get; }

            public ModuleBuilder AddModule<TDiscordModule>()
                where TDiscordModule : class, IDiscordModule
            {
                Services.AddSingleton<IDiscordModule, TDiscordModule>();

                return this;
            }

            public ModuleBuilder AddModule<TDiscordModule, TDiscordModuleOptions>(
                Action<TDiscordModuleOptions> configureOptions)
                where TDiscordModule : class, IDiscordModule
                where TDiscordModuleOptions : DiscordModuleOptions
            {
                Services
                    .AddSingleton<IDiscordModule, TDiscordModule>()
                    .Configure(configureOptions);

                return this;
            }

            public ModuleBuilder AddModule<TDiscordModule, TDiscordModuleOptions>(
                IConfiguration configuration)
                where TDiscordModule : class, IDiscordModule
                where TDiscordModuleOptions : DiscordModuleOptions
            {
                Services
                    .AddSingleton<IDiscordModule, TDiscordModule>()
                    .Configure<TDiscordModuleOptions>(configuration);

                return this;
            }

            public ModuleBuilder AddModule<TDiscordModule, TDiscordModuleOptions>(
                string name,
                IConfiguration configuration)
                where TDiscordModule : class, IDiscordModule
                where TDiscordModuleOptions : DiscordModuleOptions
            {
                Services
                    .AddSingleton<IDiscordModule, TDiscordModule>()
                    .Configure<TDiscordModuleOptions>(name, configuration);

                return this;
            }
        }
    }
}