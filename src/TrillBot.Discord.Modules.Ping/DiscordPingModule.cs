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

        public void Initialize()
        {
            _client.MessageReceived += async message =>
            {
                if (message.Content != "!ping") return;
                if (!((SocketGuildUser) message.Author).GuildPermissions.ManageGuild) return;
                await message.Channel.SendMessageAsync("pong!");
            };
        }
    }
}