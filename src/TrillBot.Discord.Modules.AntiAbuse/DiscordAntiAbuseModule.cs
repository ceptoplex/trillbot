using System.Threading.Tasks;
using Discord.WebSocket;

namespace TrillBot.Discord.Modules.AntiAbuse
{
    internal sealed class DiscordAntiAbuseModule : IDiscordModule
    {
        public const string MessagingTag = "Anti-Abuse";

        private readonly BotImpersonationMonitoring _botImpersonationMonitoring;
        private readonly DiscordSocketClient _client;
        private readonly DiscordGuildUserAvailability _guildUserAvailability;
        private readonly JoinMonitoring _joinMonitoring;

        public DiscordAntiAbuseModule(
            DiscordSocketClient client,
            DiscordGuildUserAvailability guildUserAvailability,
            BotImpersonationMonitoring botImpersonationMonitoring,
            JoinMonitoring joinMonitoring)
        {
            _client = client;
            _guildUserAvailability = guildUserAvailability;
            _botImpersonationMonitoring = botImpersonationMonitoring;
            _joinMonitoring = joinMonitoring;
        }

        public void Initialize()
        {
            _client.Ready += async () =>
            {
                // We need to use the Ready event for this because we need all users to be available
                // and fetching users somehow doesn't work well with the GuildAvailable events.
                foreach (var guild in _client.Guilds)
                    await OnGuildAvailable(guild);
            };
            _client.JoinedGuild += OnGuildAvailable;
            _client.UserJoined += async user =>
            {
                if (await _botImpersonationMonitoring.AddUserAsync(user)) return;
                await _joinMonitoring.AddUserAsync(user);
            };
            _client.GuildMemberUpdated += async (oldUser, newUser) =>
            {
                if (oldUser.Username == newUser.Username &&
                    oldUser.Nickname == newUser.Nickname)
                    return;

                await _botImpersonationMonitoring.AddUserAsync(newUser);
            };

            async Task OnGuildAvailable(SocketGuild guild)
            {
                await _guildUserAvailability.EnsureAllUsersAvailableAsync(guild);
                foreach (var user in guild.Users)
                    await _botImpersonationMonitoring.AddUserAsync(user);
            }
        }
    }
}