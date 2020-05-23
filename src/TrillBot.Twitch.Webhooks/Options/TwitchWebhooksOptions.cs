using System;

namespace TrillBot.Twitch.Webhooks.Options
{
    public sealed class TwitchWebhooksOptions
    {
        public const string Key = "Twitch:Webhooks";

        public Uri CallbackPublicUrl { get; set; }
    }
}