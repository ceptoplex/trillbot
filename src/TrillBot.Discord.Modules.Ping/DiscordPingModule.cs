using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace TrillBot.Discord.Modules.Ping
{
    internal sealed class DiscordPingModule : IDiscordModule
    {
        private readonly DiscordSocketClient _client;

        public DiscordPingModule(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _client.MessageReceived += async message =>
            {
                if (message.Content != "!ping") return;
                if (!((SocketGuildUser) message.Author).GuildPermissions.ManageGuild) return;
                await message.Channel.SendMessageAsync("pong!");
            };

            return Task.CompletedTask;
        }
    }
}