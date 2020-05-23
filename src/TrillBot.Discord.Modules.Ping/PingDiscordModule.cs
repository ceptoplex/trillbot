using Discord.WebSocket;

namespace TrillBot.Discord.Modules.Ping
{
    internal sealed class PingDiscordModule : IDiscordModule
    {
        private readonly DiscordSocketClient _discordClient;

        public PingDiscordModule(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public void Initialize()
        {
            _discordClient.MessageReceived += async message =>
            {
                if (message.Content != "!ping") return;
                if (!((SocketGuildUser) message.Author).GuildPermissions.ManageGuild) return;
                await message.Channel.SendMessageAsync("pong!");
            };
        }
    }
}