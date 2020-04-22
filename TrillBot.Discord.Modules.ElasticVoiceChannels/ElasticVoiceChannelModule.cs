using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels
{
    public class ElasticVoiceChannelModule : IModule
    {
        private readonly DiscordSocketClient _discordClient;

        private readonly IDictionary<IGuild, List<IVoiceChannel>> _guildAwaitedChannel
            = new Dictionary<IGuild, List<IVoiceChannel>>();

        private readonly IDictionary<SocketGuild, IDictionary<IChannel, IList<IVoiceChannel>>> _guildVoiceChannelGroups
            = new Dictionary<SocketGuild, IDictionary<IChannel, IList<IVoiceChannel>>>();

        private readonly ElasticVoiceChannelsOptions _options;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ElasticVoiceChannelModule(
            DiscordSocketClient discordClient,
            IOptions<ElasticVoiceChannelsOptions> options)
        {
            _discordClient = discordClient;
            _options = options.Value;
        }

        public void Initialize()
        {
            _discordClient.Ready += async () =>
            {
                foreach (var guild in _discordClient.Guilds)
                    await InitializeGuildAsync(guild);
            };
            _discordClient.JoinedGuild += async guild => await InitializeGuildAsync(guild);
            _discordClient.ChannelCreated += async channel =>
            {
                if (!(channel is SocketVoiceChannel))
                    return;
                var guild = _discordClient.Guilds.First(
                    _ => _.VoiceChannels.Any(
                        voiceChannels => voiceChannels.Id == channel.Id));

                await _semaphore.WaitAsync();
                _guildAwaitedChannel[guild].TryRemoveFirst(_ => _.Id == channel.Id);
                _semaphore.Release();

                await UpdateVoiceChannelGroupsAsync(guild);
            };
            _discordClient.ChannelDestroyed += async channel =>
            {
                if (!(channel is SocketVoiceChannel voiceChannel))
                    return;
                var guild = _guildVoiceChannelGroups.First(
                    voiceChannelGroup => voiceChannelGroup.Value.Any(
                        voiceChannels => voiceChannels.Value.Any(
                            _ => _.Id == voiceChannel.Id))).Key;

                await _semaphore.WaitAsync();
                _guildAwaitedChannel[guild].TryRemoveFirst(_ => _.Id == channel.Id);
                _semaphore.Release();

                await UpdateVoiceChannelGroupsAsync(guild);
            };
            _discordClient.ChannelUpdated += async (oldChannel, newChannel) =>
            {
                if (!(oldChannel is SocketVoiceChannel) && !(newChannel is SocketVoiceChannel))
                    return;
                var guild = _discordClient.Guilds.First(
                    _ => _.VoiceChannels.Any(
                        voiceChannels => voiceChannels.Id == newChannel.Id));

                await _semaphore.WaitAsync();
                _guildAwaitedChannel[guild].TryRemoveFirst(_ => _.Id == oldChannel.Id);
                _semaphore.Release();

                await UpdateVoiceChannelGroupsAsync(guild);
            };
            _discordClient.UserVoiceStateUpdated += async (user, oldState, newState) =>
            {
                if (newState.VoiceChannel != null &&
                    oldState.VoiceChannel != null &&
                    newState.VoiceChannel.Id == oldState.VoiceChannel.Id)
                    return;
                var stateVoiceChannel = newState.VoiceChannel ?? oldState.VoiceChannel;
                var guild = _discordClient.Guilds.First(
                    _ => _.VoiceChannels.Any(
                        voiceChannel => voiceChannel.Id == stateVoiceChannel.Id));

                await UpdateVoiceChannelGroupsAsync(guild);
            };
        }

        private async Task InitializeGuildAsync(SocketGuild guild)
        {
            _guildAwaitedChannel[guild] = new List<IVoiceChannel>();
            await UpdateVoiceChannelGroupsAsync(guild);
        }

        private async Task UpdateVoiceChannelGroupsAsync(SocketGuild guild)
        {
            if (_guildAwaitedChannel[guild].Any())
                return;

            _guildVoiceChannelGroups[guild] = new Dictionary<IChannel, IList<IVoiceChannel>>();
            IVoiceChannel currentVoiceChannelGroup = null;
            string currentVoiceChannelGroupName = null;
            foreach (var voiceChannel in guild.VoiceChannels.OrderBy(_ => _.Position))
            {
                if (_options.ExcludedChannelIds?.Contains(voiceChannel.Id) ?? false)
                {
                    currentVoiceChannelGroup = null;
                    currentVoiceChannelGroupName = null;
                    continue;
                }

                var (voiceChannelName, _) = ParseChannelName(voiceChannel);

                if (currentVoiceChannelGroup != null &&
                    voiceChannelName == currentVoiceChannelGroupName &&
                    voiceChannel.Bitrate == currentVoiceChannelGroup.Bitrate &&
                    voiceChannel.CategoryId == currentVoiceChannelGroup.CategoryId &&
                    voiceChannel.UserLimit == currentVoiceChannelGroup.UserLimit)
                {
                    _guildVoiceChannelGroups[guild][currentVoiceChannelGroup].Add(voiceChannel);
                    continue;
                }

                currentVoiceChannelGroup = voiceChannel;
                currentVoiceChannelGroupName = voiceChannelName;
                _guildVoiceChannelGroups[guild][currentVoiceChannelGroup] = new List<IVoiceChannel> {voiceChannel};
            }

            if (await RemoveNextUnnecessaryVoiceChannelAsync(guild)) return;
            if (await AddNextRequiredVoiceChannelAsync(guild)) return;
            await FixNextVoiceChannelIndexErrorAsync(guild);
        }

        private async Task<bool> RemoveNextUnnecessaryVoiceChannelAsync(SocketGuild guild)
        {
            foreach (var voiceChannels in _guildVoiceChannelGroups[guild].Values)
            {
                var emptyVoiceChannels = voiceChannels
                    .Where(_ => ((SocketVoiceChannel) _).Users.Count == 0)
                    .ToList();
                if (emptyVoiceChannels.Count < 2)
                    continue;

                _guildAwaitedChannel[guild].Add(emptyVoiceChannels.Last());
                await emptyVoiceChannels.Last().DeleteAsync();

                return true;
            }

            return false;
        }

        private async Task<bool> AddNextRequiredVoiceChannelAsync(SocketGuild guild)
        {
            foreach (var voiceChannels in _guildVoiceChannelGroups[guild].Values)
            {
                if (voiceChannels.Count >= 10)
                    // This is a fail-safe circuit to stop the creation of new channels if anything unforeseen happens.
                    continue;

                var emptyVoiceChannels = voiceChannels
                    .Where(_ => ((SocketVoiceChannel) _).Users.Count == 0)
                    .ToList();
                if (emptyVoiceChannels.Count > 0)
                    continue;
                var (name, _) = ParseChannelName(voiceChannels.First());

                await _semaphore.WaitAsync();

                var allExceptCreatedVoiceChannels = guild.VoiceChannels
                    .OrderBy(_ => _.Position)
                    .ToList();

                var createdVoiceChannel = await guild.CreateVoiceChannelAsync(
                    $"{name} ({voiceChannels.Count + 1})");
                _guildAwaitedChannel[guild].Add(createdVoiceChannel);

                _guildAwaitedChannel[guild].Add(createdVoiceChannel);
                await createdVoiceChannel.ModifyAsync(
                    properties =>
                    {
                        var voiceChannel = voiceChannels.First();
                        properties.Bitrate = voiceChannel.Bitrate;
                        properties.CategoryId = voiceChannel.CategoryId;
                        properties.UserLimit = voiceChannel.UserLimit;
                    });

                var reorderedVoiceChannels = allExceptCreatedVoiceChannels
                    .TakeWhile(_ => _.Id != voiceChannels.Last().Id)
                    .Append(voiceChannels.Last())
                    .Append(createdVoiceChannel)
                    .Concat(
                        allExceptCreatedVoiceChannels
                            .SkipWhile(_ => _.Id != voiceChannels.Last().Id)
                            .Skip(1))
                    .Select((_, i) => (Index: i, Element: _))
                    .Where(_ => _.Element.Position != _.Index)
                    .ToList();
                _guildAwaitedChannel[guild].AddRange(
                    reorderedVoiceChannels
                        .Select(_ => _.Element));
                await guild.ReorderChannelsAsync(
                    reorderedVoiceChannels
                        .Select(_ => new ReorderChannelProperties(_.Element.Id, _.Index)));

                _semaphore.Release();

                return true;
            }

            return false;
        }

        private async Task<bool> FixNextVoiceChannelIndexErrorAsync(SocketGuild guild)
        {
            foreach (var voiceChannels in _guildVoiceChannelGroups[guild].Values
                .SelectMany(_ => _)
                .Select(
                    _ =>
                    {
                        var (name, index) = ParseChannelName(_);
                        return (Name: name, Index: index, Channel: _);
                    })
                .GroupBy(_ => _.Name)
                .Select(_ => _.ToList()))
                for (var i = 0; i < voiceChannels.Count; i++)
                {
                    if (voiceChannels.Count == 1 && i == 0 && voiceChannels[i].Index == null ||
                        (voiceChannels.Count > 1 || i > 0) && voiceChannels[i].Index == i + 1)
                        continue;

                    _guildAwaitedChannel[guild].Add(voiceChannels[i].Channel);
                    var indexString = voiceChannels.Count > 1 || i > 0
                        ? $" ({i + 1})"
                        : "";
                    await voiceChannels[i].Channel.ModifyAsync(
                        properties => properties.Name = $"{voiceChannels[i].Name}{indexString}");

                    return true;
                }

            return false;
        }

        private static (string Name, int? Index) ParseChannelName(IChannel channel)
        {
            var match = Regex.Match(channel.Name, @"^(?<name>.+?)( \((?<index>\d+)\))?$");
            var name = match.Groups["name"].Value;
            var index = match.Groups["index"].Success ? int.Parse(match.Groups["index"].Value) : (int?) null;
            return (name, index);
        }
    }
}