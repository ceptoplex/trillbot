using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Discord;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels
{
    internal class ChannelGroups<TChannel>
        where TChannel : class, IChannel
    {
        private readonly IDictionary<TChannel, ICollection<TChannel>> _groups =
            new Dictionary<TChannel, ICollection<TChannel>>();

        private bool _isCurrentGroupOpen;

        private ChannelGroups()
        {
        }

        private TChannel CurrentGroup => _isCurrentGroupOpen
            ? _groups.Keys.Last()
            : null;

        public static ChannelGroups<TChannel> CreateFrom(
            IEnumerable<TChannel> channels,
            Func<TChannel, bool> excludeChannel,
            Func<TChannel, TChannel, bool> channelsEqual)
        {
            var channelGroups = new ChannelGroups<TChannel>();
            foreach (var channel in channels)
            {
                if (excludeChannel(channel))
                {
                    channelGroups.CloseGroup();
                    continue;
                }

                if (channelGroups.CurrentGroup != null &&
                    channelsEqual(channel, channelGroups.CurrentGroup))
                {
                    channelGroups.AddGroupChannel(channel);
                    continue;
                }

                channelGroups.OpenGroup(channel);
            }

            return channelGroups;
        }

        private void OpenGroup(TChannel channel)
        {
            if (CurrentGroup != null)
                CloseGroup();

            _groups[channel] = new Collection<TChannel>
            {
                channel
            };
            _isCurrentGroupOpen = true;
        }

        private void CloseGroup()
        {
            _isCurrentGroupOpen = false;
        }

        private void AddGroupChannel(TChannel channel)
        {
            if (CurrentGroup == null)
                throw new InvalidOperationException("Currently, there is no open group.");

            _groups[CurrentGroup].Add(channel);
        }

        public IEnumerable<ICollection<TChannel>> GetGroupsChannels()
        {
            return _groups.Values;
        }
    }
}