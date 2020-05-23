using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TrillBot.Twitch.Api.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public sealed class TwitchResponseWrapperDto<TData>
    {
        public IEnumerable<TData> Data { get; set; }
    }
}