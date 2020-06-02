using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels
{
    internal sealed class DiscordElasticVoiceChannelModule : IDiscordModule
    {
        private readonly IDictionary<IGuild, Awaiter<ulong>> _channelFixResultAwaiters
            = new Dictionary<IGuild, Awaiter<ulong>>();

        private readonly DiscordSocketClient _client;
        private readonly DiscordElasticVoiceChannelsModuleOptions _options;

        public DiscordElasticVoiceChannelModule(
            DiscordSocketClient client,
            IOptions<DiscordElasticVoiceChannelsModuleOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _client.GuildAvailable += OnGuildAvailable;
            _client.JoinedGuild += OnGuildAvailable;

            _client.ChannelCreated += OnChannelModified;
            _client.ChannelDestroyed += OnChannelModified;
            _client.ChannelUpdated += (oldChannel, newChannel) => OnChannelModified(oldChannel);

            _client.UserVoiceStateUpdated += OnUserVoiceStateUpdated;

            async Task OnGuildAvailable(SocketGuild guild)
            {
                _channelFixResultAwaiters[guild] = new Awaiter<ulong>();
                await FixNextChannelAsync(guild);
            }

            async Task OnChannelModified(SocketChannel channel)
            {
                if (!(channel is SocketVoiceChannel))
                    return;

                var guild = ((SocketGuildChannel) channel).Guild;
                await _channelFixResultAwaiters[guild].StopWaitingForAsync(channel.Id);
                await FixNextChannelAsync(guild);
            }

            async Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
            {
                if (newState.VoiceChannel != null &&
                    oldState.VoiceChannel != null &&
                    newState.VoiceChannel.Id == oldState.VoiceChannel.Id)
                    return;

                var channel = newState.VoiceChannel ?? oldState.VoiceChannel;
                await FixNextChannelAsync(channel.Guild);
            }

            return Task.CompletedTask;
        }

        private async Task FixNextChannelAsync(SocketGuild guild)
        {
            if (_channelFixResultAwaiters[guild].CurrentlyWaiting)
                // We are still waiting for the result of another fix.
                return;

            // We only fix the next issue, so if one method returns with a successful initiated fix,
            // we stop doing more work. Further fixes are triggered by the resulting events
            // of the currently applied fix.
            if (await FixNextUnnecessaryChannelAsync(guild)) return;
            if (await FixNextMissingChannelAsync(guild)) return;
            await FixNextIncorrectChannelNameAsync(guild);
        }

        private ChannelGroups<SocketVoiceChannel> GetChannelGroupsForElasticity(SocketGuild guild)
        {
            return GetChannelGroups(
                guild,
                (channelA, channelB) =>
                    IndexedChannelName.Create(channelA).Name ==
                    IndexedChannelName.Create(channelB).Name &&
                    channelA.Bitrate == channelB.Bitrate &&
                    channelA.CategoryId == channelB.CategoryId &&
                    channelA.UserLimit == channelB.UserLimit);
        }

        private ChannelGroups<SocketVoiceChannel> GetChannelGroupsForIndexing(SocketGuild guild)
        {
            return GetChannelGroups(
                guild,
                (channelA, channelB) =>
                    IndexedChannelName.Create(channelA).Name ==
                    IndexedChannelName.Create(channelB).Name);
        }

        private ChannelGroups<SocketVoiceChannel> GetChannelGroups(
            SocketGuild guild,
            Func<IVoiceChannel, IVoiceChannel, bool> channelsEqual)
        {
            return ChannelGroups<SocketVoiceChannel>.CreateFrom(
                guild.VoiceChannels.OrderBy(_ => _.Position),
                channel => _options.ExcludedChannelIds?.Contains(channel.Id) ?? false,
                channelsEqual);
        }

        private async Task<bool> FixNextUnnecessaryChannelAsync(SocketGuild guild)
        {
            foreach (var channels in GetChannelGroupsForElasticity(guild).GetGroupsChannels())
            {
                var emptyChannels = channels
                    .Where(_ => _.Users.Count == 0)
                    .ToList();
                if (emptyChannels.Count <= 1)
                    continue;

                await _channelFixResultAwaiters[guild].WaitForAsync(async () =>
                {
                    var channel = emptyChannels.Last();
                    await channel.DeleteAsync();
                    return channel.Id;
                });

                return true;
            }

            return false;
        }

        private async Task<bool> FixNextMissingChannelAsync(SocketGuild guild)
        {
            foreach (var channels in GetChannelGroupsForElasticity(guild).GetGroupsChannels())
            {
                var maxGroupChannelCount = _options.MaxGroupChannelCount;
                if (maxGroupChannelCount.HasValue && channels.Count >= maxGroupChannelCount)
                    continue;

                var emptyChannels = channels
                    .Where(_ => _.Users.Count == 0)
                    .ToList();
                if (emptyChannels.Count > 0)
                    continue;

                await _channelFixResultAwaiters[guild].WaitForAsync(() =>
                    CreateChannelAsync(channels));

                return true;
            }

            return false;

            async IAsyncEnumerable<ulong> CreateChannelAsync(ICollection<SocketVoiceChannel> channels)
            {
                var groupBaseChannel = channels.First();

                // Create the channel.
                var createdChannel = await guild.CreateVoiceChannelAsync(
                    IndexedChannelName.CreateWithGroupContext(
                        groupBaseChannel,
                        channels.Count,
                        channels.Count + 1).Format(),
                    properties =>
                    {
                        properties.Bitrate = groupBaseChannel.Bitrate;
                        properties.CategoryId = groupBaseChannel.CategoryId;
                        properties.UserLimit = groupBaseChannel.UserLimit;
                    });
                yield return createdChannel.Id;

                // Copy permissions to newly created channel.
                foreach (var overwrite in groupBaseChannel.PermissionOverwrites)
                {
                    if (createdChannel.PermissionOverwrites.Contains(overwrite)) continue;

                    switch (overwrite.TargetType)
                    {
                        case PermissionTarget.Role:
                            await createdChannel.AddPermissionOverwriteAsync(
                                guild.GetRole(overwrite.TargetId),
                                overwrite.Permissions);
                            break;
                        case PermissionTarget.User:
                            await createdChannel.AddPermissionOverwriteAsync(
                                _client.GetUser(overwrite.TargetId),
                                overwrite.Permissions);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    yield return createdChannel.Id;
                }

                // Calculate the list of channels with the newly created one inserted at the correct position.
                var allExceptCreatedChannels = guild.VoiceChannels
                    .OrderBy(_ => _.Position)
                    .Where(_ => _.Id != createdChannel.Id)
                    .ToList<IVoiceChannel>();
                var groupPreviouslyLastChannel = channels.Last();
                var allChannels = allExceptCreatedChannels
                    // All channels before the newly created one.
                    .TakeWhile(_ => _.Id != groupPreviouslyLastChannel.Id)
                    .Append(groupPreviouslyLastChannel)
                    // The newly created channel.
                    .Append(createdChannel)
                    // All channels after the newly created one.
                    .Concat(
                        allExceptCreatedChannels
                            .SkipWhile(_ => _.Id != groupPreviouslyLastChannel.Id)
                            .Skip(1));

                // Reorder channels that are not in the correct order after including the newly created one.
                var reorderedChannels = allChannels
                    .Select((_, i) => (index: i, channel: _))
                    .Where(_ => _.channel.Position != _.index)
                    .ToList();
                await guild.ReorderChannelsAsync(reorderedChannels
                    .Select(_ => new ReorderChannelProperties(_.channel.Id, _.index)));
                foreach (var channel in reorderedChannels
                    .Select(_ => _.channel))
                    yield return channel.Id;
            }
        }

        private async Task<bool> FixNextIncorrectChannelNameAsync(SocketGuild guild)
        {
            foreach (var (channel, groupIndex, groupCount) in GetChannelGroupsForIndexing(guild)
                .GetGroupsChannels()
                .Select(channels => channels
                    .Select((channel, i) => (
                        channel,
                        groupIndex: i,
                        groupCount: channels.Count)))
                .SelectMany(_ => _))
            {
                var existingChannelName = IndexedChannelName.Create(channel);
                var correctChannelName = IndexedChannelName.CreateWithGroupContext(
                    channel,
                    groupIndex,
                    groupCount);
                if (existingChannelName == correctChannelName)
                    continue;

                await _channelFixResultAwaiters[guild].WaitForAsync(async () =>
                {
                    await channel.ModifyAsync(properties => properties.Name = correctChannelName.Format());
                    return channel.Id;
                });

                return true;
            }

            return false;
        }
    }
}