using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrillBot.Common.Converters;

namespace TrillBot.Twitch.Api.Entities.Authentication
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class TwitchAppAccessTokenDto
    {
        public string AccessToken { get; set; }

        [JsonConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan ExpiresIn { get; set; }
    }
}