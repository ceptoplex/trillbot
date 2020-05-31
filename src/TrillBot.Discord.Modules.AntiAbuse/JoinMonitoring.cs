using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Localization;
using TrillBot.Discord.Modules.AntiAbuse.Confusables;

namespace TrillBot.Discord.Modules.AntiAbuse
{
    internal sealed class JoinMonitoring
    {
        private const int MaxRecentlyJoinedUsersWithoutWarning = 10;
        private const int MaxRecentlyJoinedConfusableUsers = 3;

        private readonly ConfusablesDetection _confusablesDetection;
        private readonly IStringLocalizer<JoinMonitoring> _localizer;
        private readonly DiscordMessaging _messaging;

        private readonly IDictionary<IGuild, ICollection<JoinedUser>> _recentlyJoinedUsers =
            new Dictionary<IGuild, ICollection<JoinedUser>>();

        public JoinMonitoring(
            ConfusablesDetection confusablesDetection,
            IStringLocalizer<JoinMonitoring> localizer,
            DiscordMessaging messaging)
        {
            _confusablesDetection = confusablesDetection;
            _localizer = localizer;
            _messaging = messaging;
        }

        public async Task<bool> AddUserAsync(IGuildUser user)
        {
            var newlyJoinedUser = new JoinedUser(user);
            var newlyJoinedUserName = newlyJoinedUser.User.Nickname ?? newlyJoinedUser.User.Username;
            var recentlyJoinedUsers = GetRecentlyJoinedUsers(newlyJoinedUser.User.Guild);

            var previouslyJoinedUser = recentlyJoinedUsers
                .Where(_ => _.Joined.HasValue)
                .OrderByDescending(_ => _.Joined.Value)
                .FirstOrDefault();

            if (previouslyJoinedUser != null &&
                !previouslyJoinedUser.JoinedWith(newlyJoinedUser))
                recentlyJoinedUsers.Clear();

            if (recentlyJoinedUsers.All(_ => _.User.Id != newlyJoinedUser.User.Id))
                recentlyJoinedUsers.Add(newlyJoinedUser);

            if (recentlyJoinedUsers.Count == MaxRecentlyJoinedUsersWithoutWarning + 1)
                await _messaging.LogGuildAsync(
                    newlyJoinedUser.User.Guild,
                    DiscordAntiAbuseModule.MessagingTag,
                    $"{_localizer["Warning"]}");

            var recentlyJoinedConfusableUsers = new List<JoinedUser>();
            foreach (var recentlyJoinedUser in recentlyJoinedUsers)
            {
                var recentlyJoinedUserName = recentlyJoinedUser.User.Nickname ?? recentlyJoinedUser.User.Username;
                if (await _confusablesDetection.TestConfusabilityAsync(recentlyJoinedUserName, newlyJoinedUserName))
                    recentlyJoinedConfusableUsers.Add(recentlyJoinedUser);
            }

            if (recentlyJoinedConfusableUsers.Count == MaxRecentlyJoinedConfusableUsers + 1)
            {
                await _messaging.LogGuildAsync(
                    user.Guild,
                    DiscordAntiAbuseModule.MessagingTag,
                    $"{_localizer["UserWasBanned", newlyJoinedUserName]}");

                foreach (var recentlyJoinedConfusableUser in recentlyJoinedConfusableUsers)
                    await BanUserAsync(recentlyJoinedConfusableUser.User);
                return true;
            }

            if (recentlyJoinedConfusableUsers.Count > MaxRecentlyJoinedConfusableUsers + 1)
            {
                await BanUserAsync(newlyJoinedUser.User);
                return true;
            }

            return false;
        }

        private ICollection<JoinedUser> GetRecentlyJoinedUsers(IGuild guild)
        {
            if (!_recentlyJoinedUsers.ContainsKey(guild))
                _recentlyJoinedUsers[guild] = new List<JoinedUser>();
            return _recentlyJoinedUsers[guild];
        }

        private async Task BanUserAsync(IGuildUser user)
        {
            await user.BanAsync(reason: $"[{DiscordAntiAbuseModule.MessagingTag}] {_localizer["UserBanReason"]}");
        }

        private sealed class JoinedUser
        {
            private static readonly TimeSpan SimultaneousJoinTimeframe = TimeSpan.FromMinutes(5);

            public JoinedUser(IGuildUser user)
            {
                User = user;
                Joined = DateTime.UtcNow;
            }

            public IGuildUser User { get; }
            public DateTime? Joined { get; }

            public bool JoinedWith(JoinedUser user)
            {
                if (!Joined.HasValue || !user.Joined.HasValue)
                    return false;
                return Joined - user.Joined <= SimultaneousJoinTimeframe;
            }
        }
    }
}