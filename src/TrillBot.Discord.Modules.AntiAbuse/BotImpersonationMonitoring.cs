using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Localization;
using TrillBot.Discord.Modules.AntiAbuse.Confusables;

namespace TrillBot.Discord.Modules.AntiAbuse
{
    public class BotImpersonationMonitoring
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfusablesDetection _confusablesDetection;
        private readonly IStringLocalizer<BotImpersonationMonitoring> _localizer;
        private readonly DiscordMessaging _messaging;

        public BotImpersonationMonitoring(
            DiscordSocketClient client,
            DiscordMessaging messaging,
            IStringLocalizer<BotImpersonationMonitoring> localizer)
        {
            _client = client;
            _messaging = messaging;
            _localizer = localizer;
            _confusablesDetection = new ConfusablesDetection(new ConfusablesCache());
        }

        public async Task<bool> AddUserAsync(IGuildUser user)
        {
            var bot = await user.Guild.GetUserAsync(_client.CurrentUser.Id);
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
                DiscordAntiAbuseModule.MessagingTag,
                $"{_localizer["UserWasKicked", user.Mention, userName, bot.Mention]}\n" +
                $"*{(notified ? _localizer["UserWasNotified"] : _localizer["UserWasNotNotified"])}*");

            // Kick afterwards, or messaging the user will not work anymore.
            await user.KickAsync(
                $"[{DiscordAntiAbuseModule.MessagingTag}] {_localizer["UserKickReason", userName, botName]}");
            return true;
        }
    }
}