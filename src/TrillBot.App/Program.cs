using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrillBot.App.Options.Discord;
using TrillBot.Discord;
using TrillBot.Discord.Extensions;
using TrillBot.Discord.Modules.AntiAbuse.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;
using TrillBot.Discord.Modules.Extensions;
using TrillBot.Discord.Modules.Options;
using TrillBot.Discord.Modules.Ping.Extensions;
using TrillBot.Discord.Options;

namespace TrillBot.App
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
            await _serviceProvider
                .GetRequiredService<DiscordBot<DiscordModulesOptions>>()
                .RunAsync(cancellationTokenSource.Token);
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
            const string resourcesPath = "Resources";

            var services = new ServiceCollection();

            // Logging
            var loggingSection = _configuration.GetSection(
                "Logging");
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(loggingSection);
                builder.AddConsole();
            });

            // Localization:
            // For now, only German language is supported because
            // the Discord server that uses this bot is a German one as well.
            services.AddLocalization(options => options.ResourcesPath = resourcesPath);
            var culture = new CultureInfo("de-DE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            ConfigureDiscordServices(services);

            return services.BuildServiceProvider();
        }

        private void ConfigureDiscordServices(IServiceCollection services)
        {
            // Bot
            var discordSection = _configuration.GetSection(DiscordOptions<DiscordModulesOptions>.Name);
            services.AddDiscordBot<DiscordModulesOptions>(discordSection);

            // Bot: Modules
            var modulesSection = discordSection.GetSection(ModulesOptions.Name);
            var elasticVoiceChannelsSection = modulesSection.GetSection(ElasticVoiceChannelsOptions.Name);
            services
                .AddDiscordModules(modulesSection)
                .AddAntiAbuseDiscordModule()
                .AddElasticVoiceChannelsDiscordModule(elasticVoiceChannelsSection)
                .AddPingDiscordModule();
        }

        internal static async Task Main()
        {
            await new Program().RunAsync();
        }
    }
}