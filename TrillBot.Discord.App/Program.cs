using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.App.Options;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;
using TrillBot.Discord.Modules.Options;
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

            var discordOptionsSection = _configuration.GetSection(DiscordOptions.Name);
            var modulesOptionsSection = discordOptionsSection.GetSection(
                ModulesOptions.Name);
            var elasticVoiceChannelsOptionsSection = modulesOptionsSection.GetSection(
                ElasticVoiceChannelsOptions.Name);

            services
                .Configure<DiscordOptions>(discordOptionsSection)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<Bootstrapper>();
            services
                .AddPingModule()
                .AddElasticVoiceChannelsModule(elasticVoiceChannelsOptionsSection);

            return services.BuildServiceProvider();
        }

        internal static async Task Main()
        {
            await new Program().RunAsync();
        }
    }
}