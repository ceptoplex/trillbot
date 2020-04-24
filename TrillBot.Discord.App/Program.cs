using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrillBot.Discord.App.Extensions;
using TrillBot.Discord.App.Options;
using TrillBot.Discord.Modules.Ping.Extensions;

namespace TrillBot.Discord.App
{
    internal class Program
    {
        private const string EnvironmentEnvironmentVariable = "NETCORE_ENVIRONMENT";
        private const string EnvironmentDefault = "Development";

        private const string ConfigurationEnvironmentVariablePrefix = "TRILLBOT:";

        private readonly IConfiguration _configuration;
        private readonly string _environment;
        private readonly IServiceProvider _serviceProvider;

        private Program()
        {
            _environment = DetectEnvironment();
            _configuration = CreateConfiguration();
            _serviceProvider = CreateServiceProvider();
        }

        private async Task RunAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                args.Cancel = true;
                cancellationTokenSource.Cancel();
            };
            await _serviceProvider.GetRequiredService<Bootstrapper>().RunAsync(cancellationTokenSource.Token);
        }

        private static string DetectEnvironment()
        {
            return Environment.GetEnvironmentVariable(EnvironmentEnvironmentVariable) ?? EnvironmentDefault;
        }

        private IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .AddYamlFile("appsettings.yml", false, true)
                .AddYamlFile($"appsettings.{_environment}.yml", false, true)
                .AddEnvironmentVariables(ConfigurationEnvironmentVariablePrefix)
                .Build();
        }

        private IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            // Logging
            var loggingSection = _configuration.GetSection(
                "Logging");
            services
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(loggingSection);
                    builder.AddConsole();
                });

            // Bot
            var discordSection = _configuration.GetSection(
                DiscordOptions.Name);
            services.AddBootstrapper(discordSection);

            // Bot: Modules
            services
                .AddPingModule();

            return services.BuildServiceProvider();
        }

        internal static async Task Main()
        {
            await new Program().RunAsync();
        }
    }
}