using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrillBot.Discord.App.Extensions;
using TrillBot.Discord.App.Options;
using TrillBot.Discord.Modules;

namespace TrillBot.Discord.App
{
    internal class Bootstrapper
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly DiscordOptions _discordOptions;
        private readonly ILogger<Bootstrapper> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Bootstrapper(
            DiscordSocketClient discordClient,
            IOptions<DiscordOptions> discordOptions,
            IServiceProvider serviceProvider,
            ILogger<Bootstrapper> logger)
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