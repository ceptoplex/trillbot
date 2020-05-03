using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrillBot.Discord.Options;

namespace TrillBot.Discord
{
    public class Messaging
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly DiscordOptions _options;

        public Messaging(DiscordSocketClient discordClient, IOptions<DiscordOptions> options)
        {
            _discordClient = discordClient;
            _options = options.Value;
        }

        public async Task LogGuildAsync(IGuild guild, string tag, string text, Embed embed = default)
        {
            if (_options.LogChannelIds == null)
                return;

            foreach (var channel in _options.LogChannelIds
                .Select(_ => _discordClient.GetChannel(_)))
            {
                if (((SocketGuildChannel) channel).Guild.Id != guild.Id)
                    return;

                await ((SocketTextChannel) channel).SendMessageAsync(
                    $"**{tag.ToUpperInvariant()}**\n" +
                    $"{text}",
                    embed: embed);
            }
        }

        public async Task<bool> LogUserAsync(IUser user, string text, Embed embed = default)
        {
            try
            {
                await user.SendMessageAsync(text, embed: embed);
                return true;
            }
            catch (HttpException e) when (e.DiscordCode == 50007)
            {
                // Cannot send direct message to this user, probably because of too strict privacy settings.
                return false;
            }
        }
    }
}