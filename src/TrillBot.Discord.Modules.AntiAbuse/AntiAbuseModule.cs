using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Localization;
using TrillBot.Discord.Modules.AntiAbuse.Confusables;

namespace TrillBot.Discord.Modules.AntiAbuse
{
    public class AntiAbuseModule : IModule
    {
        private const string MessagingTag = "Anti-Abuse";

        private readonly ConfusablesDetection _confusablesDetection;
        private readonly DiscordSocketClient _discordClient;
        private readonly GuildUserAvailability _guildUserAvailability;
        private readonly IStringLocalizer<AntiAbuseModule> _localizer;
        private readonly Messaging _messaging;

        public AntiAbuseModule(
            DiscordSocketClient discordClient,
            GuildUserAvailability guildUserAvailability,
            Messaging messaging,
            IStringLocalizer<AntiAbuseModule> localizer)
        {
            _discordClient = discordClient;
            _guildUserAvailability = guildUserAvailability;
            _messaging = messaging;
            _localizer = localizer;
            _confusablesDetection = new ConfusablesDetection(new ConfusablesCache());
        }

        public void Initialize()
        {
            _discordClient.Ready += async () =>
            {
                // We need to use the Ready event for this because we need all users to be available
                // and fetching users somehow doesn't work well with the GuildAvailable events.
                foreach (var guild in _discordClient.Guilds)
                    await OnGuildAvailable(guild);
            };
            _discordClient.JoinedGuild += OnGuildAvailable;
            _discordClient.UserJoined += OnGuildMemberChanged;
            _discordClient.GuildMemberUpdated += async (oldUser, newUser) =>
            {
                if (oldUser.Username == newUser.Username &&
                    oldUser.Nickname == newUser.Nickname)
                    return;

                await OnGuildMemberChanged(newUser);
            };

            async Task OnGuildAvailable(SocketGuild guild)
            {
                await _guildUserAvailability.EnsureAllUsersAvailableAsync(guild);
                foreach (var user in guild.Users)
                    await PreventAbuseAsync(user);
            }

            async Task OnGuildMemberChanged(IGuildUser user)
            {
                await PreventAbuseAsync(user);
            }
        }

        private async Task PreventAbuseAsync(IGuildUser user)
        {
            await PreventBotImpersonationAsync(user);
        }

        private async Task<bool> PreventBotImpersonationAsync(IGuildUser user)
        {
            var bot = await user.Guild.GetUserAsync(_discordClient.CurrentUser.Id);
            if (user.Id == bot.Id)
                return false;

            var botName = bot.Nickname ?? bot.Username;
            var userName = user.Nickname ?? user.Username;

            if (!await _confusablesDetection.TestConfusabilityAsync(botName, userName))
                return false;

            var notified = await _messaging.LogUserAsync(
                user,
                _localizer["YouWereKicked", bot.Guild.Name, userName]);
            await _messaging.LogGuildAsync(
                bot.Guild,
                MessagingTag,
                $"{_localizer["UserWasKicked", user.Mention, userName, bot.Mention]}\n" +
                $"*{(notified ? _localizer["UserWasNotified"] : _localizer["UserWasNotNotified"])}*");

            // Kick afterwards, or messaging the user will not work anymore.
            await user.KickAsync($"[{MessagingTag}] {_localizer["UserKickReason", userName, botName]}");
            return true;
        }
    }
}