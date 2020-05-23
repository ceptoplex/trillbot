using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TrillBot.Twitch.Webhooks.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    internal sealed class TwitchWebhooksContentWrapperDto<TTwitchWebhooksContentDto>
    {
        [Required]
        public IEnumerable<TTwitchWebhooksContentDto> Data { get; set; }
    }
}