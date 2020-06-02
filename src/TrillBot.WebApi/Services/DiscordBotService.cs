using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TrillBot.Discord;

namespace TrillBot.WebApi.Services
{
    internal sealed class DiscordBotService : IHostedService
    {
        private readonly DiscordBot _discordBot;

        public DiscordBotService(DiscordBot discordBot)
        {
            _discordBot = discordBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _discordBot.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordBot.StopAsync(cancellationToken);
        }
    }
}