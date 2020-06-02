using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrillBot.Discord.Extensions;
using TrillBot.Discord.Options;

namespace TrillBot.Discord
{
    public sealed class DiscordBot
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly ILogger<DiscordBot> _logger;
        private readonly DiscordOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public DiscordBot(
            DiscordSocketClient discordClient,
            IOptions<DiscordOptions> options,
            IServiceProvider serviceProvider,
            ILogger<DiscordBot> logger)
        {
            _discordClient = discordClient;
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        public bool Connected => _discordClient.ConnectionState == ConnectionState.Connected;

        public async Task StartAsync(CancellationToken cancellationToken = default)
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

            foreach (var module in _serviceProvider.GetServices<IDiscordModule>())
                await module.InitializeAsync(cancellationToken);

            await _discordClient.LoginAsync(TokenType.Bot, _options.Token);
            await _discordClient.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _discordClient.StopAsync();
        }
    }
}