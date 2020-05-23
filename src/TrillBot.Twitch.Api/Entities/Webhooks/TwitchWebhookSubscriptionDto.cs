using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TrillBot.Twitch.Api.Entities.Webhooks
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class TwitchWebhookSubscriptionDto
    {
        public Uri Topic { get; set; }
        public Uri Callback { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}