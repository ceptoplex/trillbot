using System;
using System.Text.RegularExpressions;
using TrillBot.Common.Extensions;

namespace TrillBot.Twitch.Webhooks
{
    internal static class TwitchWebhooksTopics
    {
        private static readonly Uri StreamChangedTopic = new Uri("https://api.twitch.tv/helix/streams");

        public static Uri CreateStreamChanged(string userId)
        {
            return StreamChangedTopic.Append(query: $"user_id={userId}");
        }

        public static string GetUserIdFromStreamChanged(Uri streamChangedTopic)
        {
            var result = streamChangedTopic.ToString().Substring(StreamChangedTopic.ToString().Length);
            var match = Regex.Match(result, "^user_id=(?<userId>.*)$");
            return match.Success
                ? match.Groups["userId"].Value
                : null;
        }
    }
}