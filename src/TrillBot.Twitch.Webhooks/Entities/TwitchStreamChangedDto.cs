using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TrillBot.Twitch.Webhooks.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    internal sealed class TwitchStreamChangedDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        // TODO: Add more properties.
    }
}