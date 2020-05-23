using TrillBot.Twitch.Api.Api;

namespace TrillBot.Twitch.Api
{
    public interface ITwitchUnauthenticatedApiClient
    {
        internal ITwitchAuthenticationApi Authentication();
    }
}