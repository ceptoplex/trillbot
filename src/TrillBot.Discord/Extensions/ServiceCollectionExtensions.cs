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
                .AddSingleton<GuildUserAvailability>()
                .AddSingleton<Messaging>();

            var moduleBuilder = new ModuleBuilder(services);
            configureModules(moduleBuilder);

            return services;
        }

        public class ModuleBuilder
        {
            private readonly IServiceCollection _services;

            public ModuleBuilder(IServiceCollection services)
            {
                _services = services;
            }

            public ModuleBuilder AddModule<TModule>()
                where TModule : class, IModule
            {
                _services.AddSingleton<IModule, TModule>();

                return this;
            }

            public ModuleBuilder AddModule<TModule, TModuleOptions>(Action<TModuleOptions> configureOptions)
                where TModule : class, IModule
                where TModuleOptions : ModuleOptions
            {
                _services
                    .AddSingleton<IModule, TModule>()
                    .Configure(configureOptions);

                return this;
            }

            public ModuleBuilder AddModule<TModule, TModuleOptions>(IConfiguration configuration)
                where TModule : class, IModule
                where TModuleOptions : ModuleOptions
            {
                _services
                    .AddSingleton<IModule, TModule>()
                    .Configure<TModuleOptions>(configuration);

                return this;
            }

            public ModuleBuilder AddModule<TModule, TModuleOptions>(string name, IConfiguration configuration)
                where TModule : class, IModule
                where TModuleOptions : ModuleOptions
            {
                _services
                    .AddSingleton<IModule, TModule>()
                    .Configure<TModuleOptions>(name, configuration);

                return this;
            }
        }
    }
}