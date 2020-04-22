using Discord.WebSocket;

namespace TrillBot.Discord.App.Modules
{
    internal class PingModule : IModule
    {
        private readonly DiscordSocketClient _discordClient;

        public PingModule(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
        }

        public void Initialize()
        {
            _discordClient.MessageReceived += async message =>
            {
                if (message.Content != "!ping") return;
                await message.Channel.SendMessageAsync("pong!");
            };
        }
    }
}