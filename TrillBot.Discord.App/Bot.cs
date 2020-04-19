using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrillBot.Discord.App.Configuration;
using TrillBot.Discord.App.Extensions;

namespace TrillBot.Discord.App
{
    public class Bot
    {
        private readonly DiscordOptions _discordOptions;

        public Bot(IOptions<DiscordOptions> discordOptions)
        {
            _discordOptions = discordOptions.Value;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            var client = new DiscordSocketClient();
            client.MessageReceived += async message =>
            {
                if (message.Content != "!ping") return;
                await message.Channel.SendMessageAsync("pong!");
            };

            await client.LoginAsync(TokenType.Bot, _discordOptions.Token);
            await client.StartAsync();
            await cancellationToken.AwaitCanceled();
            await client.StopAsync();
        }
    }
}