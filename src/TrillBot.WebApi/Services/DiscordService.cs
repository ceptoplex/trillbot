using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TrillBot.Discord;

namespace TrillBot.WebApi.Services
{
    internal class DiscordService : IHostedService
    {
        private readonly DiscordBot _discordBot;

        public DiscordService(DiscordBot discordBot)
        {
            _discordBot = discordBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _discordBot.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordBot.StopAsync();
        }
    }
}