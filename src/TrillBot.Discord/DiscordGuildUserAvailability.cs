using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace TrillBot.Discord
{
    public sealed class DiscordGuildUserAvailability
    {
        private readonly IDictionary<ulong, Task> _downloadUsersTasks = new Dictionary<ulong, Task>();

        public Task EnsureAllUsersAvailableAsync(IGuild guild)
        {
            if (!_downloadUsersTasks.ContainsKey(guild.Id))
                _downloadUsersTasks[guild.Id] = guild.DownloadUsersAsync();
            return _downloadUsersTasks[guild.Id];
        }
    }
}