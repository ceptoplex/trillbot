using System;
using System.Text.RegularExpressions;
using Discord;

namespace TrillBot.Discord.Modules.ElasticVoiceChannels
{
    internal class IndexedChannelName : IEquatable<IndexedChannelName>
    {
        private static readonly Regex Pattern = new Regex(@"^(?<name>.+?)( \((?<index>\d+)\))?$");

        public IndexedChannelName(string name, int? index)
        {
            Name = name;
            Index = index;
        }

        public string Name { get; }
        public int? Index { get; }

        public bool Equals(IndexedChannelName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Index == other.Index;
        }

        public string Format()
        {
            var indexString = Index.HasValue ? $" ({Index + 1})" : "";
            return $"{Name}{indexString}";
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is IndexedChannelName other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Index);
        }

        public static bool operator ==(IndexedChannelName left, IndexedChannelName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IndexedChannelName left, IndexedChannelName right)
        {
            return !Equals(left, right);
        }

        public static IndexedChannelName Create(IChannel channel)
        {
            var (name, index) = ParseChannelName(channel);
            return new IndexedChannelName(name, index);
        }

        public static IndexedChannelName CreateWithGroupContext(IChannel channel, int groupIndex, int groupCount)
        {
            return new IndexedChannelName(
                ParseChannelName(channel).Name,
                groupCount > 1 || groupIndex > 0
                    ? groupIndex
                    : (int?) null);
        }

        private static (string Name, int? Index) ParseChannelName(IChannel channel)
        {
            var match = Pattern.Match(channel.Name);
            var nameGroup = match.Groups["name"];
            var indexGroup = match.Groups["index"];
            return (nameGroup.Value,
                indexGroup.Success
                    ? int.Parse(indexGroup.Value) - 1
                    : (int?) null);
        }
    }
}