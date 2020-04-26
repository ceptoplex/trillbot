using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrillBot.Discord.Extensions;
using TrillBot.Discord.Modules;
using TrillBot.Discord.Modules.Options;
using TrillBot.Discord.Options;

namespace TrillBot.Discord
{
    public class DiscordBot<TModulesOptions>
        where TModulesOptions : ModulesOptions
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly DiscordOptions<TModulesOptions> _discordOptions;
        private readonly ILogger<DiscordBot<TModulesOptions>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DiscordBot(
            DiscordSocketClient discordClient,
            IOptions<DiscordOptions<TModulesOptions>> discordOptions,
            IServiceProvider serviceProvider,
            ILogger<DiscordBot<TModulesOptions>> logger)
        {
            _discordClient = discordClient;
            _serviceProvider = serviceProvider;
            _discordOptions = discordOptions.Value;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            _discordClient.Log += message =>
            {
                _logger.Log(
                    message.Severity.ToLogLevel(),
                    message.Exception,
                    message.Message,
                    new {message.Source});
                return Task.CompletedTask;
            };

            foreach (var module in _serviceProvider.GetRequiredService<IEnumerable<IModule>>()) module.Initialize();

            await _discordClient.LoginAsync(TokenType.Bot, _discordOptions.Token);
            await _discordClient.StartAsync();
            await cancellationToken.AwaitCanceled();
            await _discordClient.StopAsync();
        }
    }
}