using RestEase;

namespace TrillBot.Twitch.Api.Api.Authenticated
{
    public interface ITwitchAuthenticatedApi
    {
        [Header("Client-Id")]
        public string ClientId { get; set; }
    }
}