using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Modules.Options;

namespace TrillBot.Discord.Modules.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
                where TModuleOptions : class, IModuleOptions
            {
                _services
                    .AddSingleton<IModule, TModule>()
                    .Configure(configureOptions);

                return this;
            }

            public ModuleBuilder AddModule<TModule, TModuleOptions>(IConfiguration configuration)
                where TModule : class, IModule
                where TModuleOptions : class, IModuleOptions
            {
                _services
                    .AddSingleton<IModule, TModule>()
                    .Configure<TModuleOptions>(configuration);

                return this;
            }
        }
    }
}