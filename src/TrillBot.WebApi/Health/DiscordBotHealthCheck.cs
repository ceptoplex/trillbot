using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrillBot.Discord;

namespace TrillBot.WebApi.Health
{
    public class DiscordBotHealthCheck : IHealthCheck
    {
        private readonly DiscordBot _discordBot;

        public DiscordBotHealthCheck(DiscordBot discordBot)
        {
            _discordBot = discordBot;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(_discordBot.Connected
                ? HealthCheckResult.Healthy("Connected.")
                : HealthCheckResult.Unhealthy("Not connected."));
        }
    }
}